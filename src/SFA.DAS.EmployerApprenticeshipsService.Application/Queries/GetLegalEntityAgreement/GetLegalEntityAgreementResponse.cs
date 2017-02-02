using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement
{
    public class GetLegalEntityAgreementResponse
    {
        public EmployerAgreementView EmployerAgreement { get; set; }
    }
}