using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ExamenLenguajes2.API.Database.Entities
{
	public class UserEntity : IdentityUser
	{
		[StringLength(150, MinimumLength = 5)]
		[Required]
		public string FullName { get; set; }

		[StringLength(450)]
		public string RefreshToken { get; set; }

		public DateTime RefreshTokenExpire { get; set; }
	}
}
