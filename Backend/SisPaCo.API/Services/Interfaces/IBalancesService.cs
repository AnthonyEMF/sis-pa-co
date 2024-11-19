using ExamenLenguajes2.API.Dtos.Balances;
using ExamenLenguajes2.API.Dtos.Common;

namespace ExamenLenguajes2.API.Services.Interfaces
{
	public interface IBalancesService
	{
		Task<ResponseDto<PaginationDto<List<BalanceDto>>>> GetAllBalancesAsync(string searchTerm = "", int page = 1);
	}
}
