using ExamenLenguajes2.API.Database.Configuration;
using ExamenLenguajes2.API.Database.Entities;
using ExamenLenguajes2.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamenLenguajes2.API.Database
{
	public class SisPaCoContext : IdentityDbContext<UserEntity>
	{
		private readonly IAuditService _auditService;

		public SisPaCoContext(DbContextOptions<SisPaCoContext> options, IAuditService auditService) : base(options)
        {
			this._auditService = auditService;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.HasDefaultSchema("security");
			modelBuilder.Entity<UserEntity>().ToTable("users");
			modelBuilder.Entity<IdentityRole>().ToTable("roles");
			modelBuilder.Entity<IdentityUserRole<string>>().ToTable("users_roles");
			modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("users_claims");
			modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("roles_claims");
			modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("users_logins");
			modelBuilder.Entity<IdentityUserToken<string>>().ToTable("users_tokens");

			// Configurations
			modelBuilder.ApplyConfiguration(new TransactionConfiguration());
			modelBuilder.ApplyConfiguration(new EntryConfiguration());
			modelBuilder.ApplyConfiguration(new AccountConfiguration());
			modelBuilder.ApplyConfiguration(new BalanceConfiguration());

			// Set FKs OnRestrict (Evitar cascada)
			var eTypes = modelBuilder.Model.GetEntityTypes();
			foreach (var type in eTypes)
			{
				var foreignKeys = type.GetForeignKeys();
				foreach (var foreignKey in foreignKeys)
				{
					foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
				}
			}
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (
				e.State == EntityState.Added || e.State == EntityState.Modified
			));

			foreach (var entry in entries)
			{
				var entity = entry.Entity as BaseEntity;
				if (entity != null)
				{
					if (entry.State == EntityState.Added)
					{
						entity.CreatedBy = _auditService.GetUserId();
						entity.CreatedDate = DateTime.Now;
					}
					else
					{
						entity.UpdatedBy = _auditService.GetUserId();
						entity.UpdatedDate = DateTime.Now;
					}
				}
			}

			return base.SaveChangesAsync(cancellationToken);
		}

		public DbSet<TransactionEntity> Transactions { get; set; }
		public DbSet<EntryEntity> Entries { get; set; }
		public DbSet<AccountEntity> Accounts { get; set; }
		public DbSet<BalanceEntity> Balances { get; set; }
	}
}
