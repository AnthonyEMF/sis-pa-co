namespace ExamenLenguajes2.API.Dtos.Logs
{
	public class LogDto
	{
		public Guid Id { get; set; }
		public string Action { get; set; }
		public string Description { get; set; }
		public string User { get; set; }
		public DateTime Date { get; set; }
	}
}
