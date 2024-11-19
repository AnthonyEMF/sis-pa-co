using ExamenLenguajes2.API.Dtos.Entries;

namespace ExamenLenguajes2.API.Dtos.Transactions
{
	public class TransactionDto
	{
		public Guid Id { get; set; }
        public int Number { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
		public string Description { get; set; }
		public decimal TotalDebit { get; set; }
		public decimal TotalCredit { get; set; }
        public bool IsActive { get; set; }

		// Mostrar las Entradas de la partida
		public IEnumerable<EntryDto> Entries { get; set; }
	}
}
