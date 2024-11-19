using AutoMapper;
using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Database;
using ExamenLenguajes2.API.Database.Entities;
using ExamenLenguajes2.API.Dtos.Accounts;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExamenLenguajes2.API.Services
{
	public class AccountsService : IAccountsService
	{
		private readonly SisPaCoContext _context;
		private readonly ILogsService _logsService;
		private readonly IMapper _mapper;
		private readonly ILogger<AccountsService> _logger;

		private readonly int PAGE_SIZE;

		public AccountsService(
			SisPaCoContext context,
			ILogsService logsService,
			IMapper mapper, 
			ILogger<AccountsService> logger, 
			IConfiguration configuration)
        {
			this._context = context;
			this._logsService = logsService;
			this._mapper = mapper;
			this._logger = logger;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<AccountDto>>>> PaginationAccountsAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;
			var accountsEntityQuery = _context.Accounts.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				accountsEntityQuery = accountsEntityQuery
					.Where(a => (a.Name + " " + a.Code)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalAccounts = await accountsEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalAccounts / PAGE_SIZE);

			var accountsEntity = await accountsEntityQuery
				.OrderBy(a => a.Code)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var accountsDto = _mapper.Map<List<AccountDto>>(accountsEntity);

			return new ResponseDto<PaginationDto<List<AccountDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<AccountDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalAccounts,
					TotalPages = totalPages,
					Items = accountsDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages
				}
			};
		}

		public async Task<ResponseDto<List<AccountDto>>> GetAllAccountsAsync()
		{
			var accountsEntity = await _context.Accounts
				.OrderBy(a => a.Code)
				.ToListAsync();

			var accountsDto = _mapper.Map<List<AccountDto>>(accountsEntity);

			return new ResponseDto<List<AccountDto>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = accountsDto
			};
		}

		public async Task<ResponseDto<AccountDto>> GetAccountByIdAsync(Guid id)
		{
			var accountEntity = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

			if (accountEntity is null)
			{
				return new ResponseDto<AccountDto>
				{
					StatusCode = 404,
					Status = false,
					Message = MessagesConstant.RECORD_NOT_FOUND
				};
			}

			var transactionDto = _mapper.Map<AccountDto>(accountEntity);

			return new ResponseDto<AccountDto>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORD_FOUND,
				Data = transactionDto
			};
		}

		public async Task<ResponseDto<AccountDto>> CreateAccountAsync(AccountCreateDto dto)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					// Validar que no exista una cuenta con el mismo nombre
					bool nameExists = await _context.Accounts.AnyAsync(a => a.Name == dto.Name);
					if (nameExists)
					{
						return new ResponseDto<AccountDto>
						{
							StatusCode = 400,
							Status = false,
							Message = "Ya existe una cuenta con el mismo nombre."
						};
					}

					string accountCode = string.Empty;

					if (dto.ParentId.HasValue)
					{
						// Obtener la cuenta padre
						var parentAccount = await _context.Accounts
							.FirstOrDefaultAsync(a => a.Id == dto.ParentId);

						// Validar si el padre existe
						if (parentAccount == null)
						{
							return new ResponseDto<AccountDto>
							{
								StatusCode = 404,
								Status = false,
								Message = "La cuenta padre no existe."
							};
						}

						// Actualizar el allowMovement de la cuenta padre
						if (parentAccount.AllowMovement)
						{
							parentAccount.AllowMovement = false;
							_context.Accounts.Update(parentAccount);
							await _context.SaveChangesAsync();
						}

						// Generar codigo de la nueva cuenta
						accountCode = await GenerateChildAccountCode(parentAccount);
					}
					else
					{
						// Si no tiene parentId, generamos un codigo de cuenta base
						accountCode = await GenerateBaseAccountCode();
					}

					// Crear la nueva cuenta
					var newAccount = new AccountEntity
					{
						Id = Guid.NewGuid(),
						Code = accountCode,
						Name = dto.Name,
						AllowMovement = false,
						ParentId = dto.ParentId,
						IsActive = true
					};

					// Guardamos 
					_context.Accounts.Add(newAccount);
					await _context.SaveChangesAsync();

					// Registrar en los logs
					await _logsService.LogActionAsync("Creación", $"Se creó la cuenta: {newAccount.Name}");

					// Actualizar el campo AllowMovement si la cuenta no tiene hijos
					bool hasChildren = await _context.Accounts.AnyAsync(a => a.ParentId == newAccount.Id);
					newAccount.AllowMovement = !hasChildren;

					// Crear el saldo (Balance) correspondiente de la cuenta
					var initialBalance = new BalanceEntity
					{
						Id = $"{DateTime.Now.Month:D2}{DateTime.Now.Year}{accountCode}",
						Month = DateTime.Now.Month,
						Year = DateTime.Now.Year,
						AccountId = newAccount.Id,
						BalanceAmount = 0.00m
					};

					// Guardamos
					_context.Balances.Add(initialBalance);
					await _context.SaveChangesAsync();

					var accountDto = _mapper.Map<AccountDto>(newAccount);

					await transaction.CommitAsync();

					return new ResponseDto<AccountDto>
					{
						StatusCode = 201,
						Status = true,
						Message = "La cuenta ha sido creada.",
						Data = accountDto
					};
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					_logger.LogError(ex, "Error al crear la cuenta.");
					return new ResponseDto<AccountDto>
					{
						StatusCode = 500,
						Status = false,
						Message = "Hubo un error al crear la cuenta."
					};
				}
			}
		}

		public async Task<ResponseDto<AccountDto>> ToggleAccountStatusAsync(AccountEditDto dto, Guid id)
		{
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					var account = await _context.Accounts
						.Include(a => a.Balance)
						.Include(a => a.Children)
						.FirstOrDefaultAsync(a => a.Id == id);

					if (account == null)
					{
						return new ResponseDto<AccountDto>
						{
							StatusCode = 404,
							Status = false,
							Message = "La cuenta ingresada no existe."
						};
					}

					// Validar el estado ingresado
					if (account.IsActive == dto.IsActive)
					{
						string message = account.IsActive ? "La cuenta ya esta activada." : "La cuenta ya esta desactivada.";
						return new ResponseDto<AccountDto>
						{
							StatusCode = 400,
							Status = false,
							Message = message
						};
					}

					if (!account.AllowMovement && !dto.IsActive)
					{
						return new ResponseDto<AccountDto>
						{
							StatusCode = 400,
							Status = false,
							Message = "Solo se pueden desactivar cuentas que permiten movimiento."
						};
					}

					// Cambiar estado
					account.IsActive = dto.IsActive;

					if (!account.IsActive)
					{
						account.AllowMovement = false;

						// Recalcular el saldo 
						await RecalculateBalance(account);

						// Verificar si no hay hijos activos
						if (account.ParentId.HasValue)
						{
							var parentAccount = await _context.Accounts
								.Include(a => a.Children)
								.FirstOrDefaultAsync(a => a.Id == account.ParentId.Value);

							if (parentAccount != null && parentAccount.Children.All(child => !child.IsActive))
							{
								parentAccount.AllowMovement = true;
								_context.Accounts.Update(parentAccount);
							}
						}
					}
					else
					{
						// Activando la cuenta
						account.AllowMovement = account.Children == null || !account.Children.Any(child => child.IsActive);

						// Si se activa y tiene un padre, el padre no debería permitir movimiento
						if (account.ParentId.HasValue)
						{
							var parentAccount = await _context.Accounts
								.FirstOrDefaultAsync(a => a.Id == account.ParentId.Value);

							if (parentAccount != null)
							{
								parentAccount.AllowMovement = false;
								_context.Accounts.Update(parentAccount);
							}
						}
					}

					// Guardar
					_context.Accounts.Update(account);
					await _context.SaveChangesAsync();

					// Registrar en los logs
					string accountState = account.IsActive ? "activó" : "desactivó";
					await _logsService.LogActionAsync("Edición", $"Se {accountState} la cuenta: {account.Name}");

					await transaction.CommitAsync();

					var accountDto = _mapper.Map<AccountDto>(account);

					return new ResponseDto<AccountDto>
					{
						StatusCode = 200,
						Status = true,
						Message = "Estado de la cuenta actualizado correctamente.",
						Data = accountDto
					};
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					_logger.LogError(ex, "Error al actualizar el estado de la cuenta.");
					return new ResponseDto<AccountDto>
					{
						StatusCode = 500,
						Status = false,
						Message = "Hubo un error al actualizar el estado de la cuenta."
					};
				}
			}
		}

		private async Task<string> GenerateBaseAccountCode()
		{
			// Buscar las cuentas base existentes
			var lastAccount = await _context.Accounts
				.Where(a => a.ParentId == null)
				.OrderByDescending(a => a.Code)
				.FirstOrDefaultAsync();

			// Codigo autoincrementable
			int nextCode = lastAccount == null ? 1 : int.Parse(lastAccount.Code) + 1;

			return nextCode.ToString();
		}

		private async Task<string> GenerateChildAccountCode(AccountEntity parentAccount)
		{
			// Obtener las cuentas hijo existentes
			var lastChildAccount = await _context.Accounts
				.Where(a => a.ParentId == parentAccount.Id)
				.OrderByDescending(a => a.Code)
				.FirstOrDefaultAsync();

			// Generar el siguiente codigo secuencial
			int nextCode = lastChildAccount == null ? 1 : int.Parse(lastChildAccount.Code.Substring(parentAccount.Code.Length)) + 1;

			return $"{parentAccount.Code}{nextCode:D2}";
		}

		private async Task RecalculateBalance(AccountEntity account)
		{
			var accountBalance = account.Balance;
			if (accountBalance == null) return;

			decimal balanceAmountToDistribute = accountBalance.BalanceAmount;

			// Obtener las cuentas hermanas activas de la cuenta padre
			var siblingAccounts = await _context.Accounts
				.Include(a => a.Balance)
				.Where(a => a.ParentId == account.ParentId && a.Id != account.Id && a.IsActive)
				.ToListAsync();

			if (siblingAccounts.Any())
			{
				// Distribuir el saldo entre las cuentas hermanas
				foreach (var sibling in siblingAccounts)
				{
					if (sibling.Balance != null)
					{
						sibling.Balance.BalanceAmount += balanceAmountToDistribute / siblingAccounts.Count;
						_context.Balances.Update(sibling.Balance);
					}
				}
			}

			// Resetear la cuenta a 0
			accountBalance.BalanceAmount = 0;
			_context.Balances.Update(accountBalance);

			// Guardar
			await _context.SaveChangesAsync();
		}
	}
}
