using ExamenLenguajes2.API.Constants;
using ExamenLenguajes2.API.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ExamenLenguajes2.API.Database
{
	public class SisPaCoSeeder
	{
		public static async Task LoadDataAsync(
			SisPaCoContext context,
			UserManager<UserEntity> userManager,
			RoleManager<IdentityRole> roleManager,
			ILoggerFactory loggerFactory)
		{
			try
			{
				await LoadUsersAndRolesAsync(userManager, roleManager, loggerFactory);
				await LoadAccountsAsync(context, loggerFactory);
				await LoadTransactionsAsync(context, loggerFactory);
				await LoadEntriesAsync(context, loggerFactory);
				await LoadBalancesAsync(context, loggerFactory);
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<SisPaCoSeeder>();
				logger.LogError(ex, "Error al inicializar la Data del API.");
			}
		}

		public static async Task LoadUsersAndRolesAsync(UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager, ILoggerFactory loggerFactory)
		{
			try
			{
				if (!await roleManager.Roles.AnyAsync())
				{
					await roleManager.CreateAsync(new IdentityRole(RolesConstant.USER));
				}

				if (!await userManager.Users.AnyAsync())
				{
					var firstUser = new UserEntity
					{
						Id = "2a373bd7-1829-4bb4-abb7-19da4257891d",
						Email = "user1@me.com",
						UserName = "user1@me.com",
						FullName = "Primer Usuario",
					};

					await userManager.CreateAsync(firstUser, "Temporal01*");
					await userManager.AddToRoleAsync(firstUser, RolesConstant.USER);

					var secondUser = new UserEntity
					{
						Id = "ac42ed74-9880-403e-b26a-4cc81122992c",
						Email = "user2@me.com",
						UserName = "user2@me.com",
						FullName = "Segundo Usuario",
					};

					await userManager.CreateAsync(secondUser, "Temporal01*");
					await userManager.AddToRoleAsync(secondUser, RolesConstant.USER);
				}
			}
			catch (Exception e)
			{
				var logger = loggerFactory.CreateLogger<SisPaCoSeeder>();
				logger.LogError(e.Message);
			}
		}

		public static async Task LoadAccountsAsync(SisPaCoContext context, ILoggerFactory loggerFactory)
		{
			try
			{
				var jsonFilePatch = "SeedData/accounts.json";
				var jsonContent = await File.ReadAllTextAsync(jsonFilePatch);
				var accounts = JsonConvert.DeserializeObject<List<AccountEntity>>(jsonContent);

				if (!await context.Accounts.AnyAsync())
				{
					var user = await context.Users.FirstOrDefaultAsync();

					for (int i = 0; i < accounts.Count; i++)
					{
						accounts[i].CreatedBy = user.Id;
						accounts[i].CreatedDate = DateTime.Now;
						accounts[i].UpdatedBy = user.Id;
						accounts[i].UpdatedDate = DateTime.Now;
					}
					context.AddRange(accounts);
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<SisPaCoSeeder>();
				logger.LogError(ex, "Error al ejecutar el Seed de Cuentas.");
			}
		}

		public static async Task LoadTransactionsAsync(SisPaCoContext context, ILoggerFactory loggerFactory)
		{
			try
			{
				var jsonFilePatch = "SeedData/transactions.json";
				var jsonContent = await File.ReadAllTextAsync(jsonFilePatch);
				var transactions = JsonConvert.DeserializeObject<List<TransactionEntity>>(jsonContent);

				if (!await context.Transactions.AnyAsync())
				{
					var user = await context.Users.FirstOrDefaultAsync();

					for (int i = 0; i < transactions.Count; i++)
					{
						transactions[i].CreatedBy = user.Id;
						transactions[i].CreatedDate = DateTime.Now;
						transactions[i].UpdatedBy = user.Id;
						transactions[i].UpdatedDate = DateTime.Now;
					}
					context.AddRange(transactions);
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<SisPaCoSeeder>();
				logger.LogError(ex, "Error al ejecutar el Seed de Partidas.");
			}
		}

		public static async Task LoadEntriesAsync(SisPaCoContext context, ILoggerFactory loggerFactory)
		{
			try
			{
				var jsonFilePatch = "SeedData/entries.json";
				var jsonContent = await File.ReadAllTextAsync(jsonFilePatch);
				var entries = JsonConvert.DeserializeObject<List<EntryEntity>>(jsonContent);

				if (!await context.Entries.AnyAsync())
				{
					var user = await context.Users.FirstOrDefaultAsync();

					for (int i = 0; i < entries.Count; i++)
					{
						entries[i].CreatedBy = user.Id;
						entries[i].CreatedDate = DateTime.Now;
						entries[i].UpdatedBy = user.Id;
						entries[i].UpdatedDate = DateTime.Now;
					}
					context.AddRange(entries);
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<SisPaCoSeeder>();
				logger.LogError(ex, "Error al ejecutar el Seed de Entradas.");
			}
		}

		public static async Task LoadBalancesAsync(SisPaCoContext context, ILoggerFactory loggerFactory)
		{
			try
			{
				var jsonFilePatch = "SeedData/balances.json";
				var jsonContent = await File.ReadAllTextAsync(jsonFilePatch);
				var balances = JsonConvert.DeserializeObject<List<BalanceEntity>>(jsonContent);

				if (!await context.Balances.AnyAsync())
				{
					var user = await context.Users.FirstOrDefaultAsync();

					for (int i = 0; i < balances.Count; i++)
					{
						balances[i].CreatedBy = user.Id;
						balances[i].CreatedDate = DateTime.Now;
						balances[i].UpdatedBy = user.Id;
						balances[i].UpdatedDate = DateTime.Now;
					}
					context.AddRange(balances);
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<SisPaCoSeeder>();
				logger.LogError(ex, "Error al ejecutar el Seed de Saldos.");
			}
		}
	}
}
