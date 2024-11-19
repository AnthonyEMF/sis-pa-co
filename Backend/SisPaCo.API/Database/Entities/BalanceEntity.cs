using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ExamenLenguajes2.API.Database.Entities
{
	[Table("balances", Schema = "dbo")]
	public class BalanceEntity : BaseEntity
	{
		[Key]
		[Column("id")]
		[StringLength(12)] // Combinacion de mes, año y codigo de cuenta, ej: 1120241110
		public string Id { get; set; }

		[Column("month")]
		public int Month { get; set; }

		[Column("year")]
		public int Year { get; set; }

		[Column("account_id")]
		public Guid AccountId { get; set; }
		[ForeignKey(nameof(AccountId))]
		public virtual AccountEntity Account { get; set; }

		[Precision(18, 2)]
		[Column("balance_amount")]
		public decimal BalanceAmount { get; set; }

		// Propiedades de auditoria
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}
