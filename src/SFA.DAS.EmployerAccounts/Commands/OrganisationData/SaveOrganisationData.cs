using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Commands.OrganisationData;

public sealed class SaveOrganisationData : IRequest
{
    public SaveOrganisationData(EmployerAccountOrganisationData organisationData)
    {
        OrganisationData = organisationData ?? throw new ArgumentNullException(nameof(organisationData));
    }

    public EmployerAccountOrganisationData OrganisationData { get;  }
}