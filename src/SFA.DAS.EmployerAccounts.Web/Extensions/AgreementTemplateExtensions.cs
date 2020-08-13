using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class AgreementTemplateExtensions
    {
        public static string InsetText(this AgreementTemplateViewModel agreementTemplate, ICollection<OrganisationAgreementViewModel> organisationAgreementViewModel)
        {

            bool v3Accepted = false;
            foreach(var organisation in organisationAgreementViewModel)
            {                
                if (organisation.SignedDateText != string.Empty && organisation.Template.VersionNumber == 3)
                {
                    v3Accepted = true;
                }
            }
            
            switch (agreementTemplate.VersionNumber)
            {
                case 1:
                    return string.Empty;
                case 2:
                    return "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding).";
                case 3:
                    return "This is a new agreement.";
                case 4:
                    return v3Accepted ? "This is a variation to the agreement we published 9 January 2020. You only need to accept it if you want to access incentive payments for hiring a new apprentice." : "This is a new agreement.";
                default:
                    return string.Empty;
            }
        }
    }
}