using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementResponse
    {
        public EmployerAgreementView EmployerAgreement { get; set; }
    }
}