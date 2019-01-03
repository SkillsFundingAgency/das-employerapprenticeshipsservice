using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationsForAccount
{
    public class GetOrganisationsForAccountResponse
    {
        public List<RemoveOrganisationView> Organisation { get; set; }
    }
}