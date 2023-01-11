using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Commands.OrganisationAndPayeRefData;

public sealed class SaveOrganisationAndPayeData : IAsyncRequest
{
    public SaveOrganisationAndPayeData(EmployerAccountOrganisationData organisationData, EmployerAccountPayeRefData payeRefData)
    {
        OrganisationData = organisationData ?? throw new ArgumentNullException(nameof(organisationData));
        PayeRefData = payeRefData ?? throw new ArgumentNullException(nameof(payeRefData));
    }

    public EmployerAccountOrganisationData OrganisationData { get;  }
    public EmployerAccountPayeRefData PayeRefData { get; }
}