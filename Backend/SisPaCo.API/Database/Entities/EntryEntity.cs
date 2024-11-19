using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamenLenguajes2.API.Database.Entities
{
	[Table("entries", Schema = "dbo")]
	public class EntryEntity : BaseEntity
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "El Id de la partida es requerido.")]
		[Column("transaction_id")]
		public Guid TransactionId { get; set; }
		[ForeignKey(nameof(TransactionId))]
		public virtual TransactionEntity Transaction { get; set; }

		[Required(ErrorMessage = "El Id de la cuenta es requerido.")]
		[Column("account_id")]
		public Guid AccountId { get; set; }
		[ForeignKey(nameof(AccountId))]
		public virtual AccountEntity Account { get; set; }

		[Required(ErrorMessage = "El monto es requerido.")]
		[Precision(18, 2)]
		[Column("amount")]
		public decimal Amount { get; set; }

		[Required(ErrorMessage = "El tipo de entrada es requerido.")]
		[RegularExpression("^(CRÉDITO|DÉBITO)$", ErrorMessage = "El tipo de entrada debe ser CRÉDITO o DÉBITO.")]
		[Column("type")]
		public string Type { get; set; }

		// Propiedades de auditoria
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}
