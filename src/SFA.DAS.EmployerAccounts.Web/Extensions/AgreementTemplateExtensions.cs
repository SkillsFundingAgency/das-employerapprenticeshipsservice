using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class AgreementTemplateExtensions
    {
        public static string GetInsetText(this AgreementTemplate agreementTemplate)
        {
            return agreementTemplate.VersionNumber == 2 ? "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding)." : "This is a new agreement.";
        }
    }
}