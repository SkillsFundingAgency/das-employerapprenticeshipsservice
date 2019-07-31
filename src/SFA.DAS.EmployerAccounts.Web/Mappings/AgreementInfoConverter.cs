using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Types.Models.Agreement;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System;

namespace SFA.DAS.EmployerAccounts.Web.Mappings
{
    public class AgreementInfoConverter : ITypeConverter<AccountDetailViewModel, AgreementInfoViewModel>
    {
        public AgreementInfoViewModel Convert(AccountDetailViewModel source, AgreementInfoViewModel destination, ResolutionContext context)
        {
            var newAgreementInfo = new AgreementInfoViewModel
            {
                Type = (AgreementType)Enum.Parse(typeof(AgreementType), source.AccountAgreementType.ToString(), true)
            };
       
            return newAgreementInfo;
        }
    }
}