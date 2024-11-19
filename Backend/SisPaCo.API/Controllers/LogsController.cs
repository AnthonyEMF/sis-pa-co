using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Dtos.Logs;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamenLenguajes2.API.Controllers
{
	[Route("api/logs")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class LogsController : ControllerBase
	{
		private readonly ILogsService _logsService;

		public LogsController(ILogsService logsService)
        {
			this._logsService = logsService;
		}

		[HttpGet]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<List<LogDto>>>> GetAll(string searchTerm = "", int page = 1)
		{
			var response = await _logsService.GetAllLogsAsync(searchTerm, page);
			return StatusCode(response.StatusCode, response);
		}
	}
}
