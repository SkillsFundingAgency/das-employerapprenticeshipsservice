using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;

namespace SFA.DAS.EmployerAccounts.Web.Helpers;

internal class AccountLegalEntitiesHelper
{
    private readonly IMediator _mediator;

    internal AccountLegalEntitiesHelper(IMediator mediator)
    {
        _mediator = mediator;
    }

    internal async Task<List<AccountSpecificLegalEntity>> GetAccountLegalEntities(string hashedLegalEntityId, string userIdClaim)
    {
        var accountEntities = await _mediator.Send(new GetAccountLegalEntitiesRequest
        {
            HashedLegalEntityId = hashedLegalEntityId,
            UserId = userIdClaim
        });
        return accountEntities.LegalEntities;
    }

    internal bool IsLegalEntityAlreadyAddedToAccount(List<AccountSpecificLegalEntity> accountLegalEntities, string organisationName, string organisationCode, OrganisationType organisationType)
    {
        if (organisationType == OrganisationType.Charities || organisationType == OrganisationType.CompaniesHouse || organisationType == OrganisationType.PensionsRegulator)
        {
            return accountLegalEntities.Any(x => x.Code.Equals(organisationCode.Trim(), StringComparison.CurrentCultureIgnoreCase) && x.Source == organisationType);
        }
        else
        {
            return accountLegalEntities.Any(x => x.Name.Equals(organisationName.Trim(), StringComparison.CurrentCultureIgnoreCase) && x.Source == organisationType);
        }
    }
}