using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;

public class SignEmployerAgreementCommandResponse
{
    public string LegalEntityName { get; set; } 
    public AgreementType AgreementType { get; set; }
}