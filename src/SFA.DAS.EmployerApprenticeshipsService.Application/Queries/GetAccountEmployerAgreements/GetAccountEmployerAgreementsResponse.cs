using System.Collections.Generic;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsResponse
    {
        public List<EmployerAgreementStatusDto> EmployerAgreements { get; set; }
    }
}