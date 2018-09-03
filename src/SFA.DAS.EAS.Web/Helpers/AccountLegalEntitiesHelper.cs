using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Organisation;

namespace SFA.DAS.EAS.Web.Helpers
{
    internal class AccountLegalEntitiesHelper
    {
        private readonly IMediator _mediator;

        internal AccountLegalEntitiesHelper(IMediator mediator)
        {
            _mediator = mediator;
        }

        internal async Task<List<AccountSpecificLegalEntity>> GetAccountLegalEntities(string hashedLegalEntityId, string userIdClaim)
        {
            var accountEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = hashedLegalEntityId,
                UserId = userIdClaim
            });
            return accountEntities.Entites;
        }

        internal bool IsLegalEntityAlreadyAddedToAccount(List<AccountSpecificLegalEntity> accountLegalEntities, string organisationName, string organisationCode, OrganisationType organisationType)
        {
            if (organisationType == OrganisationType.Charities || organisationType == OrganisationType.CompaniesHouse)
            {
                return accountLegalEntities.Any(x => x.Code.Equals(organisationCode.Trim(), StringComparison.CurrentCultureIgnoreCase) && x.Source == organisationType);
            }
            else
            {
                return accountLegalEntities.Any(x => x.Name.Equals(organisationName.Trim(), StringComparison.CurrentCultureIgnoreCase) && x.Source == organisationType);
            }
        }
    }
}