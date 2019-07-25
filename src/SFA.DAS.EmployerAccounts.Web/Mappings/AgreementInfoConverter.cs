using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerAccounts.Types.Models.Agreement;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Web.Mappings
{
    public class AgreementInfoConverter : ITypeConverter<AccountDetailViewModel, AgreementInfoViewModel>
    {
        public AgreementInfoViewModel Convert(AccountDetailViewModel source, AgreementInfoViewModel destination, ResolutionContext context)
        {
            // format "<agrementType>.<phase>.<version>" e.g. "NonLevy.EOI.1"
            var strings = source.AccountAgreementType.Split('.');

            var newAgreementInfo = new AgreementInfoViewModel
            {
                Type = (AgreementType)Enum.Parse(typeof(AgreementType), strings[0], true)
            };

            if (strings.Count() > 1)
            {
                newAgreementInfo.Phase = new AgreementPhase
                {
                    Type = (PhaseType)Enum.Parse(typeof(PhaseType), strings[1], true),
                    Version = strings.Count() == 3 ? System.Convert.ToInt32(strings[2]) : 0
                };
            }

            return newAgreementInfo;
        }
    }
}