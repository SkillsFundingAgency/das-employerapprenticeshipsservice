﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EmployerAccounts.Api.Mappings;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;
using SFA.DAS.EmployerAccounts.Queries.GetLegalEntity;
using SFA.DAS.Validation.WebApi;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [Route("api/accounts/{hashedAccountId}/legalentities")]
    public class LegalEntitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LegalEntitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("", Name = "GetLegalEntities")]
        [Authorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IActionResult> GetLegalEntities(string hashedAccountId, bool includeDetails = false)
        {
            GetAccountLegalEntitiesByHashedAccountIdResponse result;

            try
            {
                result = await _mediator.Send(
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
                                Href = Url.RouteUrl("GetLegalEntity",
                                    new { hashedAccountId, legalEntityId = legalEntity.LegalEntityId })
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
        public async Task<IActionResult> GetLegalEntity(string hashedAccountId, long legalEntityId,
            bool includeAllAgreements = false)
        {
            var response = await _mediator.Send(request: new GetLegalEntityQuery(hashedAccountId, legalEntityId));

            var model = LegalEntityMapping.MapFromAccountLegalEntity(response.LegalEntity, response.LatestAgreement,
                includeAllAgreements);

            return Ok(model);
        }
    }
}