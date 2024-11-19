using AutoMapper;
using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Database;
using ExamenLenguajes2.API.Database.Entities;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Dtos.Logs;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamenLenguajes2.API.Services
{
	public class LogsService : ILogsService
	{
		private readonly SisPaCoLogsContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IMapper _mapper;
		private readonly int PAGE_SIZE;

		public LogsService(SisPaCoLogsContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
        {
			this._context = context;
			this._httpContextAccessor = httpContextAccessor;
			this._mapper = mapper;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<LogDto>>>> GetAllLogsAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;
			var logsEntityQuery = _context.Logs.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				logsEntityQuery = logsEntityQuery
					.Where(l => (l.Action + " " + l.User + " " + l.Date)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalLogs = await logsEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalLogs / PAGE_SIZE);

			var logsEntity = await logsEntityQuery
				.OrderByDescending(a => a.Date)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var logsDto = _mapper.Map<List<LogDto>>(logsEntity);

			return new ResponseDto<PaginationDto<List<LogDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<LogDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalLogs,
					TotalPages = totalPages,
					Items = logsDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages
				}
			};
		}

		public async Task LogActionAsync(string action, string description)
		{
			var user = _httpContextAccessor.HttpContext?.User?.Claims.
				FirstOrDefault(x => x.Type == ClaimTypes.Email || x.Type == "email")?.Value ?? "Sistema";

			var log = new LogEntity
			{
				Id = Guid.NewGuid(),
				Action = action,
				Description = description,
				User = user,
				Date = DateTime.UtcNow
			};

			await _context.Logs.AddAsync(log);
			await _context.SaveChangesAsync();
		}
	}
}
