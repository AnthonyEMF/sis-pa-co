using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Dtos.Balances;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamenLenguajes2.API.Controllers
{
	[Route("api/balances")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class BalancesController : ControllerBase
	{
		private readonly IBalancesService _balancesService;

		public BalancesController(IBalancesService balancesService)
        {
			this._balancesService = balancesService;
		}

		[HttpGet]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<List<BalanceDto>>>> GetAll(string searchTerm = "", int page = 1)
		{
			var response = await _balancesService.GetAllBalancesAsync(searchTerm, page);
			return StatusCode(response.StatusCode, response);
		}
	}
}
