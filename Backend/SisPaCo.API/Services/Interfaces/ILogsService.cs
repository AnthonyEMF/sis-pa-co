using ExamenLenguajes2.API.Dtos.Balances;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Dtos.Logs;

namespace ExamenLenguajes2.API.Services.Interfaces
{
	public interface ILogsService
	{
		Task<ResponseDto<PaginationDto<List<LogDto>>>> GetAllLogsAsync(string searchTerm = "", int page = 1);
		Task LogActionAsync(string action, string description);
	}
}
