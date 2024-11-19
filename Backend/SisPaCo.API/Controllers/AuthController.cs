using ExamenLenguajes2.API.Dtos.Auth;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamenLenguajes2.API.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
        {
			this._authService = authService;
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<ActionResult<ResponseDto<LoginResponseDto>>> Login(LoginDto dto)
		{
			var response = await _authService.LoginAsync(dto);
			return StatusCode(response.StatusCode, response);
		}

		[HttpPost("refresh-token")]
		[AllowAnonymous]
		public async Task<ActionResult<ResponseDto<LoginResponseDto>>> RefreshToken(RefreshTokenDto dto)
		{
			var response = await _authService.RefreshTokenAsync(dto);
			return StatusCode(response.StatusCode, response);
		}
	}
}
