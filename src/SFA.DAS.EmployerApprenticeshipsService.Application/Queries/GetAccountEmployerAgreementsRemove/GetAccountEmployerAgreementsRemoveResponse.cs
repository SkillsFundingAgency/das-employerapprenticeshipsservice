using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove
{
    public class GetAccountEmployerAgreementsRemoveResponse
    {
        public List<RemoveEmployerAgreementView> Agreements { get; set; }
    }
}