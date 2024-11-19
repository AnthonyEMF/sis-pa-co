using System.ComponentModel.DataAnnotations;

namespace ExamenLenguajes2.API.Dtos.Accounts
{
	public class AccountCreateDto
	{
		public Guid? ParentId { get; set; }

        [Required(ErrorMessage = "El nombre de la cuenta es requerido.")]
		public string Name { get; set; }

	}
}
