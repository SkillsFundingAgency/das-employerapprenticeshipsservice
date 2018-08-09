using SFA.DAS.EmployerAccounts.DtosX;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementResponse
    {
        public AgreementDto EmployerAgreement { get; set; }
        public AgreementDto LastSignedAgreement { get; set; }
    }
}