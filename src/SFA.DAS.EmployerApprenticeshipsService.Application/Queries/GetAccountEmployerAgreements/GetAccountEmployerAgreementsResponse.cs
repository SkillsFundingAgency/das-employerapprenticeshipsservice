using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsResponse
    {
        public List<EmployerAgreementView> EmployerAgreements { get; set; }
    }
}