using AutoMapper;
using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Database;
using ExamenLenguajes2.API.Dtos.Balances;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExamenLenguajes2.API.Services
{
	public class BalancesService : IBalancesService
	{
		private readonly SisPaCoContext _context;
		private readonly IMapper _mapper;
		private readonly int PAGE_SIZE;

		public BalancesService(SisPaCoContext context, IMapper mapper, IConfiguration configuration)
        {
			this._context = context;
			this._mapper = mapper;
			PAGE_SIZE = configuration.GetValue<int>("PageSize");
		}

        public async Task<ResponseDto<PaginationDto<List<BalanceDto>>>> GetAllBalancesAsync(string searchTerm = "", int page = 1)
		{
			int startIndex = (page - 1) * PAGE_SIZE;
			var balancesEntityQuery = _context.Balances.Include(b => b.Account).AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				balancesEntityQuery = balancesEntityQuery
					.Where(b => (b.Account.Name + " " + b.Account.Code + " " + b.Id)
					.ToLower().Contains(searchTerm.ToLower()));
			}

			int totalBalances = await balancesEntityQuery.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalBalances / PAGE_SIZE);

			var balancesEntity = await balancesEntityQuery
				.OrderBy(b => b.Id)
				.Skip(startIndex)
				.Take(PAGE_SIZE)
				.ToListAsync();

			var balancesDto = _mapper.Map<List<BalanceDto>>(balancesEntity);

			return new ResponseDto<PaginationDto<List<BalanceDto>>>
			{
				StatusCode = 200,
				Status = true,
				Message = MessagesConstant.RECORDS_FOUND,
				Data = new PaginationDto<List<BalanceDto>>
				{
					CurrentPage = page,
					PageSize = PAGE_SIZE,
					TotalItems = totalBalances,
					TotalPages = totalPages,
					Items = balancesDto,
					HasPreviousPage = page > 1,
					HasNextPage = page < totalPages
				}
			};
		}
	}
}
