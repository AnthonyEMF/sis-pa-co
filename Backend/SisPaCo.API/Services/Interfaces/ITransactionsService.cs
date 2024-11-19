using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Dtos.Transactions;

namespace ExamenLenguajes2.API.Services.Interfaces
{
	public interface ITransactionsService
	{
		Task<ResponseDto<PaginationDto<List<TransactionDto>>>> GetAllTransactionsAsync(string searchTerm = "", int page = 1);
		Task<ResponseDto<TransactionDto>> GetTransactionByIdAsync(Guid id);
		Task<ResponseDto<TransactionDto>> CreateTransactionAsync(TransactionCreateDto dto);
		Task<ResponseDto<TransactionDto>> DeactivateTransactionAsync(TransactionEditDto dto, Guid id);
	}
}
