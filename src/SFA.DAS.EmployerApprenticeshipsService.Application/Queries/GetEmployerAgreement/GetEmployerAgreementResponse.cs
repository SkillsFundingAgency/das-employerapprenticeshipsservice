using SFA.DAS.EAS.Application.Dtos;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementResponse
    {
        public AgreementDto EmployerAgreement { get; set; }
        public AgreementDto LastSignedAgreement { get; set; }
    }
}