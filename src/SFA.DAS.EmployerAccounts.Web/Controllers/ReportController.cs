﻿using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Commands.ReportTrainingProvider;
using SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("report")]
    public class ReportController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public ReportController(IMediator mediator,
            ILog logger, IAuthenticationService owinWrapper,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("trainingprovider/{correlationId}")]
        public async Task<ActionResult> TrainingProvider(string correlationId)
        {
            if (Guid.TryParse(correlationId, out var correlationGuid))
            {
                _logger.Debug($"Reporting Training Provider with correlationId: {correlationId}");

                //If being reported, unsubscribe to not get further notifications anyway
                await _mediator.SendAsync(new UnsubscribeProviderEmailQuery
                {
                    CorrelationId = correlationGuid
                });

                var invitation = await _mediator.SendAsync(new GetProviderInvitationQuery
                {
                    CorrelationId = correlationGuid
                });

                if (invitation.Result != null)
                {
                    await _mediator.SendAsync(new ReportTrainingProviderCommand(
                        invitation.Result.EmployerEmail,
                        DateTime.Now,
                        invitation.Result.ProviderOrganisationName,
                        invitation.Result.ProviderUserFullName,
                        invitation.Result.SentDate
                        )
                    ); ;
                }
            }

            var model = new
            {
                HideHeaderSignInLink = true
            };

            return View(ControllerConstants.ReportTrainingProviderViewName, model);
        }
    }
}