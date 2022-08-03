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

            CreateMap<SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationView, SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel>()
                .ForMember(target => target.PayeSchemeReference, opt => opt.MapFrom(src => src.EmpRef));            

            CreateMap<Domain.Models.EmployerAgreement.EmployerAgreementView, EmployerAgreementView>();

            CreateMap<SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel, SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>();

            CreateMap<SFA.DAS.EAS.Finance.Api.Types.TransactionSummaryViewModel, SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>()
                .ForMember(target => target.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(target => target.Href, opt => opt.MapFrom(src => src.Href))
                .ForMember(target => target.Month, opt => opt.MapFrom(src => src.Month))
                .ForMember(target => target.Year, opt => opt.MapFrom(src => src.Year)); 

            CreateMap<SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel, SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>()
                .ForMember(target => target.HasPreviousTransactions, opt => opt.MapFrom(src => src.HasPreviousTransactions))
                .ForMember(target => target.PreviousMonthUri, opt => opt.MapFrom(src => src.PreviousMonthUri))
                .ForMember(target => target.Month, opt => opt.MapFrom(src => src.Month))
                .ForMember(target => target.Year, opt => opt.MapFrom(src => src.Year));

            CreateMap<SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel, SFA.DAS.EAS.Account.Api.Types.TransactionViewModel>()
                .ForMember(target => target.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(target => target.TransactionType, opt => opt.MapFrom(src => src.TransactionType))
                .ForMember(target => target.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(target => target.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(target => target.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(target => target.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(target => target.SubTransactions, opt => opt.MapFrom(src => src.SubTransactions))
                .ForMember(target => target.ResourceUri, opt => opt.MapFrom(src => src.ResourceUri));

            //TODO : Check below mappings

            CreateMap<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel, SFA.DAS.EAS.Finance.Api.Types.TransactionSummaryViewModel>()
              .ForMember(target => target.Amount, opt => opt.MapFrom(src => src.Amount))
              .ForMember(target => target.Href, opt => opt.MapFrom(src => src.Href))
              .ForMember(target => target.Month, opt => opt.MapFrom(src => src.Month))
              .ForMember(target => target.Year, opt => opt.MapFrom(src => src.Year));

            CreateMap<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel, SFA.DAS.EAS.Finance.Api.Types.TransactionsViewModel>()
             .ForMember(target => target.HasPreviousTransactions, opt => opt.MapFrom(src => src.HasPreviousTransactions))
             .ForMember(target => target.PreviousMonthUri, opt => opt.MapFrom(src => src.PreviousMonthUri))
             .ForMember(target => target.Month, opt => opt.MapFrom(src => src.Month))
             .ForMember(target => target.Year, opt => opt.MapFrom(src => src.Year));

            CreateMap<SFA.DAS.EAS.Account.Api.Types.TransactionViewModel, SFA.DAS.EAS.Finance.Api.Types.TransactionViewModel>()
                .ForMember(target => target.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(target => target.TransactionType, opt => opt.MapFrom(src => src.TransactionType))
                .ForMember(target => target.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(target => target.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
                .ForMember(target => target.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(target => target.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(target => target.SubTransactions, opt => opt.MapFrom(src => src.SubTransactions))
                .ForMember(target => target.ResourceUri, opt => opt.MapFrom(src => src.ResourceUri));
        }
    }
}