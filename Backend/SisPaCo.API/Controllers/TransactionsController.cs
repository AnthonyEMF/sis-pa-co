using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Dtos.Transactions;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamenLenguajes2.API.Controllers
{
	[Route("api/transactions")]
	[ApiController]
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class TransactionsController : ControllerBase
	{
		private readonly ITransactionsService _transactionsService;

		public TransactionsController(ITransactionsService transactionsService)
		{
			this._transactionsService = transactionsService;
		}

		[HttpGet]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<List<TransactionDto>>>> GetAll(string searchTerm = "", int page = 1)
		{
			var response = await _transactionsService.GetAllTransactionsAsync(searchTerm, page);
			return StatusCode(response.StatusCode, response);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<List<TransactionDto>>>> Get(Guid id)
		{
			var response = await _transactionsService.GetTransactionByIdAsync(id);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<TransactionDto>>> Create(TransactionCreateDto dto)
		{
			var response = await _transactionsService.CreateTransactionAsync(dto);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = $"{RolesConstant.USER}")]
		public async Task<ActionResult<ResponseDto<TransactionDto>>> SoftDelete(TransactionEditDto dto, Guid id)
		{
			var response = await _transactionsService.DeactivateTransactionAsync(dto, id);
			return StatusCode(response.StatusCode, response);
		}
	}
}
