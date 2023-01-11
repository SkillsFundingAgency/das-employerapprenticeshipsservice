using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;

public class GetOrganisationAgreementsResponse
{
    public virtual ICollection<EmployerAgreementDto> Agreements { get; set; } = new List<EmployerAgreementDto>();
}