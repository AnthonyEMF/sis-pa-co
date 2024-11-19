using ExamenLenguajes2.API.Dtos.Auth;
using ExamenLenguajes2.API.Dtos.Common;
using System.Security.Claims;

namespace ExamenLenguajes2.API.Services.Interfaces
{
	public interface IAuthService
	{
		Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto);
		Task<ResponseDto<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
		ClaimsPrincipal GetTokenPrincipal(string token);
	}
}
