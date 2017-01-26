using System.Collections.Generic;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsResponse
    {
        public List<EmployerAgreementView> EmployerAgreements { get; set; }
    }
}