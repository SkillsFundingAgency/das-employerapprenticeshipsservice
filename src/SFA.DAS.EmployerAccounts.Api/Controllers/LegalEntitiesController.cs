using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
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
            var result = await _mediator.SendAsync(
                new GetEmployerAccountByHashedIdQuery
                {
                    HashedAccountId = hashedAccountId
                });

            if (result.Account == null)
            {
                return NotFound();
            }

            var resources = new List<ResourceViewModel>();

            foreach (var legalEntity in result.Account.AccountLegalEntities)
            {
                resources
                    .Add(
                        new ResourceViewModel
                        {
                            Id = legalEntity.LegalEntityId.ToString(),
                            Href = Url.Route("GetLegalEntity", new { hashedAccountId, legalEntity.LegalEntityId })
                        });
            }

            return Ok(
                new ResourceList(resources));
        }
    }
}