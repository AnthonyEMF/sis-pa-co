using System.ComponentModel.DataAnnotations;

namespace ExamenLenguajes2.API.Dtos.Entries
{
	public class EntryCreateDto
	{
		[Required(ErrorMessage = "El Id de la cuenta del catalogo es requerido.")]
		public Guid AccountId { get; set; }

		[Required(ErrorMessage = "El monto de la entrada es requerido.")]
		public decimal Amount { get; set; }

		[Required(ErrorMessage = "El tipo de entrada es requerido.")]
		[RegularExpression("^(CRÉDITO|DÉBITO)$", ErrorMessage = "El tipo de entrada debe ser CRÉDITO o DÉBITO.")]
		public string Type { get; set; } 
	}
}
