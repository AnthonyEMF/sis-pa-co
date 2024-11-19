using ExamenLenguajes2.API.Dtos.Entries;
using System.ComponentModel.DataAnnotations;

namespace ExamenLenguajes2.API.Dtos.Transactions
{
	public class TransactionEditDto
	{
		[Required(ErrorMessage = "La estado de la partida es requerido.")]
		public bool IsActive { get; set; }
	}
}
