using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Database;
using ExamenLenguajes2.API.Database.Entities;
using ExamenLenguajes2.API.Dtos.Auth;
using ExamenLenguajes2.API.Dtos.Common;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ExamenLenguajes2.API.Services
{
	public class AuthService : IAuthService
	{
		private readonly SignInManager<UserEntity> _signInManager;
		private readonly UserManager<UserEntity> _userManager;
		private readonly SisPaCoContext _context;
		private readonly ILogsService _logsService;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AuthService> _logger;

		public AuthService(
			SignInManager<UserEntity> signInManager,
			UserManager<UserEntity> userManager,
			SisPaCoContext context,
			ILogsService logsService,
			IConfiguration configuration,
			ILogger<AuthService> logger)
        {
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._context = context;
			this._logsService = logsService;
			this._configuration = configuration;
			this._logger = logger;
		}

        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto)
		{
			var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				// Capturar el usuario
				var userEntity = await _userManager.FindByEmailAsync(dto.Email);

				// Generación del Token
				List<Claim> authClaims = await GetClaims(userEntity);
				var jwtToken = GetToken(authClaims);
				var refreshToken = GenerateRefreshTokenString();

				// Pasar el Refresh Token
				userEntity.RefreshToken = refreshToken;
				userEntity.RefreshTokenExpire = DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:RefreshTokenExpire"] ?? "30"));

				// Guardar
				_context.Entry(userEntity);
				await _context.SaveChangesAsync();

				// Registrar en logs
				await _logsService.LogActionAsync("Inicio de sesión", $"El usuario {userEntity.FullName} ha iniciado sesión.");

				return new ResponseDto<LoginResponseDto>
				{
					StatusCode = 200,
					Status = true,
					Message = MessagesConstant.LOGIN_SUCCESS,
					Data = new LoginResponseDto
					{
						FullName = userEntity.FullName,
						Email = userEntity.Email,
						Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
						TokenExpiration = jwtToken.ValidTo,
						RefreshToken = refreshToken,
					}
				};
			}

			return new ResponseDto<LoginResponseDto>
			{
				Status = false,
				StatusCode = 401,
				Message = MessagesConstant.LOGIN_ERROR
			};
		}

		public async Task<ResponseDto<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
		{
			try
			{
				var principal = GetTokenPrincipal(dto.Token);
				var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

				if (emailClaim is null)
				{
					return new ResponseDto<LoginResponseDto>
					{
						StatusCode = 401,
						Status = false,
						Message = "Acceso no autorizado: No se encontró un email valido."
					};
				}

				// Obtener el email
				string email = emailClaim.Value;
				var userEntity = await _userManager.FindByEmailAsync(email);

				if (userEntity is null)
				{
					return new ResponseDto<LoginResponseDto>
					{
						StatusCode = 401,
						Status = false,
						Message = "Acceso no autorizado: El usuario no existe."
					};
				}

				if (userEntity.RefreshToken != dto.RefreshToken)
				{
					return new ResponseDto<LoginResponseDto>
					{
						StatusCode = 401,
						Status = false,
						Message = "Acceso no autorizado: La sesión no es valida."
					};
				}

				if (userEntity.RefreshTokenExpire < DateTime.Now)
				{
					return new ResponseDto<LoginResponseDto>
					{
						StatusCode = 401,
						Status = false,
						Message = "Acceso no autorizado: La sesión ha expirado."
					};
				}

				// Generación del Token
				List<Claim> authClaims = await GetClaims(userEntity);
				var JwtToken = GetToken(authClaims);

				var loginResponseDto = new LoginResponseDto
				{
					Email = email,
					FullName = userEntity.FullName,
					Token = new JwtSecurityTokenHandler().WriteToken(JwtToken),
					TokenExpiration = JwtToken.ValidTo,
					RefreshToken = GenerateRefreshTokenString()
				};

				// Pasar el refreshToken al userEntity
				userEntity.RefreshToken = loginResponseDto.RefreshToken;
				userEntity.RefreshTokenExpire = DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:RefreshTokenExpire"] ?? "30"));

				// Guardar
				_context.Entry(userEntity);
				await _context.SaveChangesAsync();

				return new ResponseDto<LoginResponseDto>
				{
					StatusCode = 200,
					Status = true,
					Message = "Token renovado satisfactoriamente.",
					Data = loginResponseDto
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return new ResponseDto<LoginResponseDto>
				{
					StatusCode = 500,
					Status = false,
					Message = "Ocurrió un error al renovar el token."
				};
			}
		}

		public ClaimsPrincipal GetTokenPrincipal(string token)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value));

			var validation = new TokenValidationParameters
			{
				IssuerSigningKey = securityKey,
				ValidateLifetime = false,
				ValidateActor = false,
				ValidateIssuer = false,
				ValidateAudience = false
			};

			return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
		}

		private async Task<List<Claim>> GetClaims(UserEntity userEntity)
		{
			var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, userEntity.Email),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim("UserId", userEntity.Id),
				};

			var userRoles = await _userManager.GetRolesAsync(userEntity);
			foreach (var role in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role));
			}

			return authClaims;
		}

		private SecurityToken GetToken(List<Claim> authClaims)
		{
			var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

			return new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddMinutes(int.Parse(_configuration["JWT:Expires"] ?? "15")),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
			);
		}

		private string GenerateRefreshTokenString()
		{
			var randomNumber = new byte[64];

			using (var numberGenerator = RandomNumberGenerator.Create())
			{
				numberGenerator.GetBytes(randomNumber);
			}

			return Convert.ToBase64String(randomNumber);
		}
	}
}
