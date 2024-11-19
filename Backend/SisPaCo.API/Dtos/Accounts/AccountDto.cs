namespace ExamenLenguajes2.API.Dtos.Accounts
{
	public class AccountDto
	{
		public Guid Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public bool AllowMovement { get; set; }
		public Guid? ParentId { get; set; }
        public bool IsActive { get; set; }
	}
}
