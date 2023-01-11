using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;

public class GetLastSignedAgreementResponse
{
    public AgreementDto LastSignedAgreement { get; set; }
}