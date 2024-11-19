using System.ComponentModel.DataAnnotations;

namespace ExamenLenguajes2.API.Dtos.Accounts
{
	public class AccountEditDto
	{
		[Required(ErrorMessage = "La estado de la partida es requerido.")]
		public bool IsActive { get; set; }
	}
}
