using AutoMapper;
using SFA.DAS.EmployerAccounts.Dtos;
using Account = SFA.DAS.EmployerAccounts.Models.Account.Account;
using AccountDetail = SFA.DAS.EmployerAccounts.Api.Types.AccountDetail;

namespace SFA.DAS.EmployerAccounts.Mappings;

public class AccountMappings : Profile
{
    public AccountMappings()
    {
        CreateMap<Account, AccountDto>();

        CreateMap<Account, AccountDetail>()
            .ForMember(target => target.AccountId, opt => opt.MapFrom(src => src.Id))
            .ForMember(target => target.HashedAccountId, opt => opt.MapFrom(src => src.HashedId))
            .ForMember(target => target.PublicHashedAccountId, opt => opt.MapFrom(src => src.PublicHashedId))
            .ForMember(target => target.DateRegistered, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(target => target.DasAccountName, opt => opt.MapFrom(src => src.Name))
            .ForMember(target => target.OwnerEmail, opt => opt.Ignore())
            .ForMember(target => target.LegalEntities, opt => opt.Ignore())
            .ForMember(target => target.PayeSchemes, opt => opt.Ignore())
            .ForMember(target => target.AccountAgreementType, opt => opt.Ignore());
    }
}