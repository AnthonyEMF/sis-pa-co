using AutoMapper;
using ExamenLenguajes2.API.Database.Entities;
using ExamenLenguajes2.API.Dtos.Accounts;
using ExamenLenguajes2.API.Dtos.Balances;
using ExamenLenguajes2.API.Dtos.Entries;
using ExamenLenguajes2.API.Dtos.Logs;
using ExamenLenguajes2.API.Dtos.Transactions;

namespace ExamenLenguajes2.API.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			MapsForBalances();
			MapsForTransactions();
			MapsForEntries();
			MapsForAccounts();
			MapsForLogs();
		}

		private void MapsForBalances()
		{
			CreateMap<BalanceEntity, BalanceDto>();
		}

		private void MapsForTransactions()
		{
			CreateMap<TransactionEntity, TransactionDto>()
						.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName)) 
						.ForMember(dest => dest.Entries, opt => opt.MapFrom(src => src.Entries));
			CreateMap<TransactionCreateDto, TransactionEntity>();
			CreateMap<TransactionEditDto, TransactionEntity>();
		}

		private void MapsForEntries()
		{
			CreateMap<EntryEntity, EntryDto>();
			CreateMap<EntryCreateDto, EntryEntity>();
			CreateMap<EntryEditDto, EntryEntity>();
		}

		private void MapsForAccounts()
		{
			CreateMap<AccountEntity, AccountDto>();
			CreateMap<AccountCreateDto, AccountEntity>();
			CreateMap<AccountEditDto, AccountEntity>();
		}

		private void MapsForLogs()
		{
			CreateMap<LogEntity, LogDto>();
		}
	}
}
