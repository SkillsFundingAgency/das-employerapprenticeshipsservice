using System;
using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Mappings
{
    public class AgreementInfoConverter : ITypeConverter<AccountDetailViewModel, AgreementInfoViewModel>
    {
        public AgreementInfoViewModel Convert(AccountDetailViewModel source, AgreementInfoViewModel destination, ResolutionContext context)
        {
            var newAgreementInfo = new AgreementInfoViewModel
            {
                Type = (AccountAgreementType)Enum.Parse(typeof(AccountAgreementType), source.AccountAgreementType.ToString(), true)
            };
       
            return newAgreementInfo;
        }
    }
}