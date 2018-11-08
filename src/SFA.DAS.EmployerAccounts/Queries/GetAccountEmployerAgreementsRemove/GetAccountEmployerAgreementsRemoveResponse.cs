using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementsRemove
{
    public class GetAccountEmployerAgreementsRemoveResponse
    {
        public List<RemoveEmployerAgreementView> Agreements { get; set; }
    }
}