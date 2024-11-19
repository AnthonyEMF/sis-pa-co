using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExamenLenguajes2.API.Database.Entities
{
	[Table("logs", Schema = "dbo")]
	public class LogEntity
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Required]
		[Column("action")]
		public string Action { get; set; } 

		[Column("description")]
		public string Description { get; set; }

		[Column("user")]
		public string User { get; set; }

		[Column("date")]
		public DateTime Date { get; set; }
	}
}
