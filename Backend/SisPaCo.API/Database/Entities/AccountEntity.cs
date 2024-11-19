using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamenLenguajes2.API.Database.Entities
{
	[Table("accounts", Schema = "dbo")]
	public class AccountEntity : BaseEntity
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "El codigo es requerido.")]
		[StringLength(20)]
		[Column("code")]
		public string Code { get; set; }

		[Required(ErrorMessage = "El nombre es requerido.")]
		[StringLength(200)]
		[Column("name")]
		public string Name { get; set; }

		[Column("allow_movement")]
		public bool AllowMovement { get; set; }

		[Column("parent_id")]
		public Guid? ParentId { get; set; }
		[ForeignKey(nameof(ParentId))]
		public virtual AccountEntity Parent { get; set; }

		[Required(ErrorMessage = "Definir el estado de la partida es requerido.")]
		[Column("is_active")]
		public bool IsActive { get; set; }

		// Relación de navegación con Balance
		public virtual BalanceEntity Balance { get; set; }

		// Propiedades de navegacion
		public virtual ICollection<AccountEntity> Children { get; set; }
		public virtual ICollection<EntryEntity> Entries { get; set; }

		// Propiedades de auditoria
		public virtual UserEntity CreatedByUser { get; set; }
		public virtual UserEntity UpdatedByUser { get; set; }
	}
}
