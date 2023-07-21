namespace SFA.DAS.EmployerAccounts.Web.Extensions;

public static class AgreementTemplateExtensions
{
    public static readonly int[] VariationsOfv3Agreement = {3, 4, 5, 6};

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
                return HasSignedVariationOfv3Agreement(organisationAgreementViewModel) ? "This is a variation to the agreement we published 9 January 2020. You only need to accept it if you want to access incentive payments for hiring a new apprentice." : "This is a new agreement.";
            case var _ when agreementTemplate.VersionNumber >= 5:
                return HasSignedVariationOfv3Agreement(organisationAgreementViewModel) ? "This is a variation to the agreement we published 9 January 2020." : "This is a new agreement.";
            default:
                return string.Empty;
        }
    }

    private static bool HasSignedVariationOfv3Agreement(ICollection<OrganisationAgreementViewModel> organisationAgreementViewModel)
    {
        return organisationAgreementViewModel
            .Any(agreement => !string.IsNullOrEmpty(agreement.SignedDateText) && VariationsOfv3Agreement.Contains(agreement.Template.VersionNumber));
    }
}