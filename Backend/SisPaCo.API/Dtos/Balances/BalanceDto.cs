using ExamenLenguajes2.API.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExamenLenguajes2.API.Dtos.Balances
{
	public class BalanceDto
	{
		// Combinacion de mes, año y codigo de cuenta, ej: 1120241110
		public string Id { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }
        public string AccountCode { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public decimal BalanceAmount { get; set; }
	}
}
