using System.Linq;
using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Account.Api.Mappings
{
    public class AccountMappings : Profile
    {
        public AccountMappings()
        {
            CreateMap<Domain.Models.Account.Account, AccountDetailViewModel>()
                .ForMember(target => target.AccountId, opt => opt.MapFrom(src => src.Id))
                .ForMember(target => target.HashedAccountId, opt => opt.MapFrom(src => src.HashedId))
                .ForMember(target => target.PublicHashedAccountId, opt => opt.MapFrom(src => src.PublicHashedId))
                .ForMember(target => target.DateRegistered, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(target => target.DasAccountName, opt => opt.MapFrom(src => src.Name));
            CreateMap<LevyDeclarationView, LevyDeclarationViewModel>()
                .ForMember(target => target.PayeSchemeReference, opt => opt.MapFrom(src => src.EmpRef));

            CreateMap<SFA.DAS.EmployerFinance.Models.Levy.LevyDeclarationView, SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel>()
                .ForMember(target => target.PayeSchemeReference, opt => opt.MapFrom(src => src.EmpRef));

            CreateMap<Domain.Models.EmployerAgreement.EmployerAgreementView, EmployerAgreementView>();

            CreateMap<SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel, SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>();

            CreateMap<SFA.DAS.EAS.Finance.Api.Types.TransactionSummaryViewModel, SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>();
            CreateMap<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel, SFA.DAS.EAS.Finance.Api.Types.TransactionSummaryViewModel>();

            CreateMap<SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel, SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>();
            CreateMap<SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel, SFA.DAS.EAS.Account.Api.Types.TransactionViewModel>();

            CreateMap<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel, SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel>();
            CreateMap<SFA.DAS.EAS.Account.Api.Types.TransactionViewModel, SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel>();
        }
    }
}