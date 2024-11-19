using AutoMapper;
using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Database;
using ExamenLenguajes2.API.Database.Entities;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Dtos.Transactions;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExamenLenguajes2.API.Services
{
	public class TransactionsService : ITransactionsService
	{
		private readonly SisPaCoContext _context;
		private readonly UserManager<UserEntity> _userManager;
		private readonly ILogsService _logsService;
		private readonly IMapper _mapper;
		private readonly ILogger<TransactionsService> _logger;
		private readonly int PAGE_SIZE;

		public TransactionsService(
			SisPaCoContext context,
			UserManager<UserEntity> userManager,
			ILogsService logsService,
			IMapper mapper,
			ILogger<TransactionsService> logger,
			IConfiguration configuration)
        {
			this._context = context;
			this._userManager = userManager;
			this._logsService = logsService;
			this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<TransactionDto>>>> GetAllTransactionsAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;
			var transactionsEntityQuery = _context.Transactions
						.Include(t => t.User).Include(t => t.Entries)
						.ThenInclude(e => e.Account)
						.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				transactionsEntityQuery = transactionsEntityQuery
					.Where(t => (t.Description + " " + t.Number + " " + t.User.FullName)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalTransactions = await transactionsEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalTransactions / PAGE_SIZE);

			var transactionsEntity = await transactionsEntityQuery
				.OrderBy(t => t.Number)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var transactionsDto = _mapper.Map<List<TransactionDto>>(transactionsEntity);

			return new ResponseDto<PaginationDto<List<TransactionDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<TransactionDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalTransactions,
					TotalPages = totalPages,
					Items = transactionsDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages
				}
			};
		}

		public async Task<ResponseDto<TransactionDto>> GetTransactionByIdAsync(Guid id)
		{
			var transactionEntity = await _context.Transactions
					.Include(t => t.User).Include(t => t.Entries).ThenInclude(e => e.Account).FirstOrDefaultAsync(t => t.Id == id);

			if (transactionEntity is null)
			{
				return new ResponseDto<TransactionDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			var transactionDto = _mapper.Map<TransactionDto>(transactionEntity);

			return new ResponseDto<TransactionDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = transactionDto
			};
		}

		public async Task<ResponseDto<TransactionDto>> CreateTransactionAsync(TransactionCreateDto dto)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
					if (user == null) // Validar que el usuario existe
					{
						return new ResponseDto<TransactionDto>
						{
							StatusCode = 404,
							Status = false,
							Message = "El usuario ingresado no existe."
						};
					}

					// Numero de la partida
					int nextTransactionNumber = await _context.Transactions
						.OrderByDescending(t => t.Number)
						.Select(t => t.Number)
						.FirstOrDefaultAsync() + 1;

					// Inicializar la nueva Partida
					var transactionEntity = new TransactionEntity
					{
						Id = Guid.NewGuid(),
						Number = nextTransactionNumber,
						UserId = dto.UserId,
						Date = DateTime.UtcNow, // Se asigna la fecha actual
						Description = dto.Description,
						IsActive = true,
						Entries = new List<EntryEntity>()
					};

					decimal totalDebit = 0;
					decimal totalCredit = 0;

					// Validar y procesar las entradas
					foreach (var entryDto in dto.Entries)
					{
						var account = await _context.Accounts.FindAsync(entryDto.AccountId);
						if (account == null) // Validar que la cuenta existe
						{
							return new ResponseDto<TransactionDto>
							{
								StatusCode = 404,
								Status = false,
								Message = "La cuenta ingresada no existe en el catalogo de cuentas."
							};
						}
						if (!account.AllowMovement) // Validar que la cuenta permite movimiento
						{
							return new ResponseDto<TransactionDto>
							{
								StatusCode = 400,
								Status = false,
								Message = "La cuenta no permite movimiento."
							};
						}

						// Crear la entrada
						var entryEntity = new EntryEntity
						{
							Id = Guid.NewGuid(),
							TransactionId = transactionEntity.Id,
							AccountId = entryDto.AccountId,
							Amount = entryDto.Amount,
							Type = entryDto.Type
						};

						// Sumar el monto al total segun el tipo
						if (entryEntity.Type == "CRÉDITO")
						{
							totalCredit += entryEntity.Amount;
						}
						else if (entryEntity.Type == "DÉBITO")
						{
							totalDebit += entryEntity.Amount;
						}
						else
						{
							return new ResponseDto<TransactionDto>
							{
								StatusCode = 400,
								Status = false,
								Message = "El tipo de entrada debe ser DÉBITO o CRÉDITO."
							};
						}

						// Agregar la entrada a la partida
						transactionEntity.Entries.Add(entryEntity);
					}

					// Validar que TotalDebit y TotalCredit cuadran
					if (totalDebit != totalCredit)
					{
						return new ResponseDto<TransactionDto>
						{
							StatusCode = 400,
							Status = false,
							Message = "El total de débitos no cuadra con el total de créditos."
						};
					}

					// Calcular y actualizar los saldos de las cuentas
					foreach (var entry in transactionEntity.Entries)
					{
						// Obtener el saldo relacionado a partir del Mes, Año y id de la cuenta
						var balance = await _context.Balances.FirstOrDefaultAsync(b =>
							b.AccountId == entry.AccountId &&
							b.Month == transactionEntity.Date.Month &&
							b.Year == transactionEntity.Date.Year);

						// Actualizar el saldo segun el tipo de la entrada
						if (entry.Type == "CRÉDITO")
						{
							balance.BalanceAmount += entry.Amount;
						}
						else if (entry.Type == "DÉBITO")
						{
							balance.BalanceAmount -= entry.Amount;
						}

						// Actualizar saldos de las cuentas padre
						var account = await _context.Accounts.FindAsync(entry.AccountId);
						if (account.ParentId != null)
						{
							await UpdateParentAccountBalance(account.ParentId.Value, entry.Type, entry.Amount);
						}
					}

					transactionEntity.TotalCredit = totalCredit;
					transactionEntity.TotalDebit = totalDebit;

					// Guardamos la partida
					_context.Transactions.Add(transactionEntity);
					await _context.SaveChangesAsync();

					// Registrar en los logs
					await _logsService.LogActionAsync("Creación", $"Se creó la partida número: {transactionEntity.Number}");

					await transaction.CommitAsync();

					var transactionDto = _mapper.Map<TransactionDto>(transactionEntity);

					return new ResponseDto<TransactionDto>
					{
						StatusCode = 201,
						Status = true,
						Message = MessagesConstant.CREATE_SUCCESS,
						Data = transactionDto
					};
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					_logger.LogError(ex, MessagesConstant.CREATE_ERROR);
					return new ResponseDto<TransactionDto>
					{
						StatusCode = 500,
						Status = false,
						Message = MessagesConstant.CREATE_ERROR
					};
				}
			}
		}

		public async Task<ResponseDto<TransactionDto>> DeactivateTransactionAsync(TransactionEditDto dto, Guid id)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var transactionEntity = await _context.Transactions.Include(t => t.Entries).FirstOrDefaultAsync(t => t.Id == id);

					if (transactionEntity == null)
					{
						return new ResponseDto<TransactionDto>
						{
							StatusCode = 404,
							Status = false,
							Message = "La partida no existe."
						};
					}

					// Si la partida ya esta desactivada, no permitir activarla nuevamente
					if (!transactionEntity.IsActive && dto.IsActive)
					{
						return new ResponseDto<TransactionDto>
						{
							StatusCode = 400,
							Status = false,
							Message = "La partida ya esta desactivada y no puede ser activada nuevamente."
						};
					}

					// Validar estado de la partida
					if (transactionEntity.IsActive == dto.IsActive)
					{
						return new ResponseDto<TransactionDto>
						{
							StatusCode = 400,
							Status = false,
							Message = "La partida ya se encuentra en el estado que se intenta cambiar."
						};
					}

					// Desactivar la partida
					if (!dto.IsActive)
					{
						transactionEntity.IsActive = false;
					}

					// Revertir los saldos si la partida se desactiva
					if (!dto.IsActive)
					{
						foreach (var entry in transactionEntity.Entries)
						{
							var account = await _context.Accounts.FindAsync(entry.AccountId);
							var balance = await _context.Balances.FirstOrDefaultAsync(b =>
								b.AccountId == entry.AccountId &&
								b.Month == transactionEntity.Date.Month &&
								b.Year == transactionEntity.Date.Year);

							if (balance != null)
							{
								if (entry.Type == "CRÉDITO")
								{
									balance.BalanceAmount -= entry.Amount;
								}
								else if (entry.Type == "DÉBITO")
								{
									balance.BalanceAmount += entry.Amount;
								}
							}

							// Revertir saldos en las cuentas padre
							if (account?.ParentId != null)
							{
								await UpdateParentAccountBalance(account.ParentId.Value, entry.Type == "CRÉDITO" ? "DÉBITO" : "CRÉDITO", entry.Amount);
							}
						}
					}

					// Guardar 
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					// Registrar en los logs
					await _logsService.LogActionAsync("Edición", $"Se desactivó la partida número: {transactionEntity.Number}");

					var transactionDto = _mapper.Map<TransactionDto>(transactionEntity);

					return new ResponseDto<TransactionDto>
					{
						StatusCode = 200,
						Status = true,
						Message = "El estado de la partida se actualizó correctamente.",
						Data = transactionDto
					};
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					_logger.LogError(ex, "Error al actualizar el estado de la partida.");
					return new ResponseDto<TransactionDto>
					{
						StatusCode = 500,
						Status = false,
						Message = "Ocurrió un error al actualizar el estado de la partida."
					};
				}
			}
		}

		private async Task UpdateParentAccountBalance(Guid parentId, string entryType, decimal amount)
		{
			var parentAccount = await _context.Accounts.FindAsync(parentId);
			if (parentAccount == null) // Si la cuenta no tiene padres (es huerfana) retornar al flujo normal
			{
				return;
			}

			var parentBalance = await _context.Balances.FirstOrDefaultAsync(b =>
				b.AccountId == parentId &&
				b.Month == DateTime.Now.Month &&
				b.Year == DateTime.Now.Year);

			if (parentBalance == null) // Si no existe el saldo de la cuenta padre, lo creamos
			{
				parentBalance = new BalanceEntity
				{
					Id = $"{parentAccount.Id}{DateTime.Now.Month}{DateTime.Now.Year}",
					Month = DateTime.Now.Month,
					Year = DateTime.Now.Year,
					AccountId = parentAccount.Id,
					BalanceAmount = 0
				};
				_context.Balances.Add(parentBalance);
			}

			// Actualizamos el saldo del balance del padre
			if (entryType == "CRÉDITO")
			{
				parentBalance.BalanceAmount += amount;
			}
			else if (entryType == "DÉBITO")
			{
				parentBalance.BalanceAmount -= amount;
			}

			await _context.SaveChangesAsync();

			// Si la cuenta padre tambien tiene un padre continuamos el ciclo
			if (parentAccount.ParentId != null)
			{
				await UpdateParentAccountBalance(parentAccount.ParentId.Value, entryType, amount);
			}
		}
	}
}
