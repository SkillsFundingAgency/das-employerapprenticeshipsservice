using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.Validation;
using SFA.DAS.Validation.WebApi;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : ApiController
    {
        private readonly IMediator _mediator;

        public LegalEntitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("", Name = "GetLegalEntities")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetLegalEntities(string hashedAccountId)
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

            if (result.Entites.Count == 0)
            {
                return NotFound();
            }

            var resources = new List<ResourceViewModel>();

            foreach (var legalEntity in result.Entites)
            {
                resources
                    .Add(
                        new ResourceViewModel
                        {
                            Id = legalEntity.Id.ToString(),
                            Href = Url.Route("GetLegalEntity", new { hashedAccountId, legalEntityId = legalEntity.Id})
                        });
            }

            return Ok(
                new ResourceList(resources));
        }

        [Route("{legalEntityId}", Name = "GetLegalEntity")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpNotFoundForNullModel]
        public async Task<IHttpActionResult> GetLegalEntity()
        {
            throw new NotImplementedException();
//            var response = await _mediator.SendAsync(query);
//            return Ok(response.LegalEntity);
        }
    }
}