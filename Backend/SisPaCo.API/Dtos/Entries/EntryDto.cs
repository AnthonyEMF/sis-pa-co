namespace ExamenLenguajes2.API.Dtos.Entries
{
	public class EntryDto
	{
		public Guid Id { get; set; }
		public Guid TransactionId { get; set; }
		public Guid AccountId { get; set; }
		public string AccountName { get; set; } // incluir el nombre de la cuenta
		public decimal Amount { get; set; }
		public string Type { get; set; } 
	}
}
