using SFA.DAS.EmployerAccounts.Models.ReferenceData;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator
{
    public class GetPensionRegulatorResponse
    {
        public IEnumerable<OrganisationName> Organisations { get; set; }
    }
}