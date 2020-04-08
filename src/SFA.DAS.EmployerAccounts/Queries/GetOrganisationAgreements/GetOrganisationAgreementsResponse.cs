using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements
{
    public class GetOrganisationAgreementsResponse
    {
        //public OrganisationAgreement OrganisationAgreements { get; set; }

        public virtual ICollection<EmployerAgreementDto> Agreements { get; set; } = new List<EmployerAgreementDto>();
    }
}