using ExamenLenguajes2.API.Dtos.Entries;
using System.ComponentModel.DataAnnotations;

namespace ExamenLenguajes2.API.Dtos.Transactions
{
	public class TransactionCreateDto
	{
		[Required(ErrorMessage = "El Id del usuario es requerido.")]
		public string UserId { get; set; }

		[Required(ErrorMessage = "La descripción de la partida es requerida.")]
		public string Description { get; set; }

		// Crear entradas de la partida
		public IEnumerable<EntryCreateDto> Entries { get; set; }
	}
}
