using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class AgreementTemplateExtensions
    {
        public const int AgreementVersionV3 = 3;
        public const int AgreementVersionV4 = 4;

        public static string InsetText(this AgreementTemplateViewModel agreementTemplate, ICollection<OrganisationAgreementViewModel> organisationAgreementViewModel)
        {

            switch (agreementTemplate.VersionNumber)
            {
                case 1:
                    return string.Empty;
                case 2:
                    return "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding).";
                case 3:
                    return "This is a new agreement.";
                case 4:
                case 5:
                    bool v3orv4Accepted = GetV3orV4AgreementStatus(organisationAgreementViewModel);
                    return v3orv4Accepted ? "This is a variation to the agreement we published 9 January 2020. You only need to accept it if you want to access incentive payments for hiring a new apprentice." : "This is a new agreement.";
                default:
                    return string.Empty;
            }
        }

        private static bool GetV3orV4AgreementStatus(ICollection<OrganisationAgreementViewModel> organisationAgreementViewModel)
        {
            bool v3Accepted = false;
            foreach (var organisation in organisationAgreementViewModel)
            {
                if (organisation.SignedDateText != string.Empty && 
                    (organisation.Template.VersionNumber == AgreementVersionV3 ||
                     organisation.Template.VersionNumber == AgreementVersionV4))
                {
                    v3Accepted = true;
                }
            }

            return v3Accepted;
        }
    }
}