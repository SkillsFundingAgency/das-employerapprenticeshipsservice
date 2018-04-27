using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementResponse
    {
        public EmployerAgreementDto EmployerAgreement { get; set; }
        public EmployerAgreementDto LastSignedAgreement { get; set; }
    }
}