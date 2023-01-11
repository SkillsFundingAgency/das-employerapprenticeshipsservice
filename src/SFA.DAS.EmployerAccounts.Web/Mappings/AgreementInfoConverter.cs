using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EmployerAccounts.Web.Mappings;

public class AgreementInfoConverter : ITypeConverter<AccountDetailViewModel, AgreementInfoViewModel>
{
    public AgreementInfoViewModel Convert(AccountDetailViewModel source, AgreementInfoViewModel destination, ResolutionContext context)
    {
        var newAgreementInfo = new AgreementInfoViewModel
        {
            Type = source.AccountAgreementType
        };
       
        return newAgreementInfo;
    }
}