using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class SignAgreementViewModel
{
    public bool HasFurtherPendingAgreements { get; set; }
    public AgreementType SignedAgreementType { get; set; }
    public string LegalEntityName { get; set; }
}