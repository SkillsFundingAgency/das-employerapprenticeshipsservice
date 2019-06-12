using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.PensionRegulator
{
    public class Organisation
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int UniqueIdentity { get; set; }
        public OrganisationType Type { get; set; }  
        public Models.Organisation.Address Address { get; set; }     
    }
}