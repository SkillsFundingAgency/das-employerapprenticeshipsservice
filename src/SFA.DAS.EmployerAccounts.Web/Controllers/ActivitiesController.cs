﻿using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Queries.GetActivities;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Models.Activities;
using SFA.DAS.EmployerAccounts.Queries.GetLatestActivities;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize("EmployerFeature.Activities", EmployerUserRole.Any)]
    [RoutePrefix("accounts/{HashedAccountId}/activity")]
    public class ActivitiesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public ActivitiesController(IMapper mapper, IMediator mediator, ILog logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        [Route]
        public async Task<ActionResult> Index(GetActivitiesQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<ActivitiesViewModel>(response);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult Latest(GetLatestActivitiesQuery query)
        {
            try
            {
                var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
                var model = _mapper.Map<LatestActivitiesViewModel>(response);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Failed to get the latest activities.");

                return Content(ControllerConstants.ActivitiesUnavailableMessage);
            }
        }
    }
}