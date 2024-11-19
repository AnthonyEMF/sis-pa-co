using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamenLenguajes2.API.Database.Entities
{
	[Table("transactions", Schema = "dbo")]
	public class TransactionEntity : BaseEntity
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Column("number")]
		public int Number { get; set; }

		[Column("user_id")]
		public string UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public virtual UserEntity User { get; set; }

		[Required(ErrorMessage = "La fecha es requerida.")]
		[Column("date")]
		public DateTime Date { get; set; }

		[StringLength(250)]
		[Required(ErrorMessage = "La descripción es requerida.")]
		[Column("description")]
		public string Description { get; set; }

		[Column("total_debit")]
		[Precision(18, 2)]
		public decimal TotalDebit { get; set; }

		[Column("total_credit")]
		[Precision(18, 2)]
		public decimal TotalCredit { get; set; }

		[Required(ErrorMessage = "Definir el estado de la partida es requerido.")]
		[Column("is_active")]
		public bool IsActive { get; set; } // En lugar de borrar la partida solo vamos a desactivarla 

		// Entradas de la Partida
		public virtual ICollection<EntryEntity> Entries { get; set; }

		// Propiedades de auditoria
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}
