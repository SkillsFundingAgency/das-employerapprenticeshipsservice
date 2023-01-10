using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Mappings;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;
using SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;
using SFA.DAS.Validation;
using SFA.DAS.Validation.WebApi;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IMediator _mediator;

        public LegalEntitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("", Name = "GetLegalEntities")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId, bool includeDetails = false)
        {
            GetAccountLegalEntitiesByHashedAccountIdResponse result;

            try
            {
                result = await _mediator.SendAsync(
                    new GetAccountLegalEntitiesByHashedAccountIdRequest
                    {
                        HashedAccountId = hashedAccountId
                    });
            }
            catch (InvalidRequestException)
            {
                return NotFound();
            }

            if (result.LegalEntities.Count == 0)
            {
                return NotFound();
            }

            if (!includeDetails)
            {
                var resources = new List<Resource>();

                foreach (var legalEntity in result.LegalEntities)
                {
                    resources
                        .Add(
                            new Resource
                            {
                                Id = legalEntity.LegalEntityId.ToString(),
                                Href = Url.Route("GetLegalEntity", new { hashedAccountId, legalEntityId = legalEntity.LegalEntityId })
                            });
                }

                return Ok(new ResourceList(resources));
            }

            return Ok(result.LegalEntities.Select(c => LegalEntityMapping.MapFromAccountLegalEntity(c, null, false))
                .ToList());
        }

        [Route("{legalEntityId}", Name = "GetLegalEntity")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpNotFoundForNullModel]
        public async Task<IHttpActionResult> GetLegalEntity(
            string hashedAccountId,
            long legalEntityId,
            bool includeAllAgreements = false)
        {
            var response = await _mediator.SendAsync(request: new GetLegalEntityQuery(hashedAccountId, legalEntityId));

            var model = LegalEntityMapping.MapFromAccountLegalEntity(response.LegalEntity, response.LatestAgreement, includeAllAgreements);
            
            return Ok(model);
        }
    }
}