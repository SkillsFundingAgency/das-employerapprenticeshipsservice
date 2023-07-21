namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class SignEmployerAgreementViewModel
{
    public const int ReviewAgreementLater = 1;
    public const int SignAgreementNow = 2;

    public EmployerAgreementView EmployerAgreement { get; set; }
    public EmployerAgreementView PreviouslySignedEmployerAgreement { get; set; }
    public int Choice { get; set; }
    public int LegalEntitiesCount { get; set; }
    public bool NoChoiceSelected { get; set; }
}