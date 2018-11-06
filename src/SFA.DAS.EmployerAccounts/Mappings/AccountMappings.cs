using AutoMapper;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Account, AccountContext>();
            CreateMap<Account, AccountDto>();
            CreateMap<Account, AccountDetailViewModel>()
                .ForMember(target => target.HashedAccountId, opt => opt.MapFrom(src => src.HashedId))
                .ForMember(target => target.DasAccountName, opt => opt.MapFrom(src => src.Name))
                .ForMember(target => target.AccountId, opt => opt.MapFrom(src => src.Id))
                .ForMember(target => target.PublicHashedAccountId, opt => opt.MapFrom(src => src.PublicHashedId))
                .ForMember(target => target.DateRegistered, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(target => target.OwnerEmail, opt => opt.Ignore())
                .ForMember(target => target.LegalEntities, opt => opt.Ignore())
                .ForMember(target => target.PayeSchemes, opt => opt.Ignore())
                .ForMember(target => target.Balance, opt => opt.Ignore())
                .ForMember(target => target.TransferAllowance, opt => opt.Ignore())
                .ForMember(target => target.RemainingTransferAllowance, opt => opt.Ignore())
                .ForMember(target => target.StartingTransferAllowance, opt => opt.Ignore());
        }
    }
}