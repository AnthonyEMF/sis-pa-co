using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Dtos.Accounts;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamenLenguajes2.API.Controllers
{
	[Route("api/accounts")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class AccountsController : ControllerBase
	{
		private readonly IAccountsService _accountsService;

		public AccountsController(IAccountsService accountsService)
        {
			this._accountsService = accountsService;
		}

		[HttpGet]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<List<AccountDto>>>> PaginationList(string searchTerm = "", int page = 1)
		{
			var response = await _accountsService.PaginationAccountsAsync(searchTerm, page);
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("all")]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<List<AccountDto>>>> GetAll()
		{
			var response = await _accountsService.GetAllAccountsAsync();
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<List<AccountDto>>>> Get(Guid id)
		{
			var response = await _accountsService.GetAccountByIdAsync(id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<AccountDto>>> Create(AccountCreateDto dto)
		{
			var response = await _accountsService.CreateAccountAsync(dto);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<AccountDto>>> Edit(AccountEditDto dto, Guid id)
		{
			var response = await _accountsService.ToggleAccountStatusAsync(dto, id);
			return StatusCode(response.StatusCode, response);
		}
	}
}
