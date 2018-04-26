using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementResponse
    {
        public EmployerAgreement EmployerAgreement { get; set; }
        public EmployerAgreement LastSignedAgreement { get; set; }
    }
}