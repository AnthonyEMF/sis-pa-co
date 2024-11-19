using ExamenLenguajes2.API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamenLenguajes2.API.Database
{
	public class SisPaCoLogsContext : DbContext
	{
        public SisPaCoLogsContext(DbContextOptions<SisPaCoLogsContext> options) : base(options)
        {
        }

		public DbSet<LogEntity> Logs { get; set; }
}
}
