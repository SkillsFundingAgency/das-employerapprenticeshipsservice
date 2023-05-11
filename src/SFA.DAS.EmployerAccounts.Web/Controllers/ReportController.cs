using SFA.DAS.EmployerAccounts.Commands.ReportTrainingProvider;
using SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Route("report")]
public class ReportController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IMediator mediator,
        ILogger<ReportController> logger,
        ICookieStorageService<FlashMessageViewModel> flashMessage)
        : base(flashMessage)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Route("trainingprovider/{correlationId}")]
    public async Task<IActionResult> TrainingProvider(string correlationId)
    {
        if (Guid.TryParse(correlationId, out var correlationGuid))
        {
            _logger.LogDebug("Reporting Training Provider with correlationId: {CorrelationId}", correlationId);

            //If being reported, unsubscribe to not get further notifications anyway
            await _mediator.Send(new UnsubscribeProviderEmailCommand
            {
                CorrelationId = correlationGuid
            });

            var invitation = await _mediator.Send(new GetProviderInvitationQuery
            {
                CorrelationId = correlationGuid
            });

            if (invitation.Result != null)
            {
                await _mediator.Send(new ReportTrainingProviderCommand(
                        invitation.Result.EmployerEmail,
                        DateTime.Now,
                        invitation.Result.ProviderOrganisationName,
                        invitation.Result.ProviderUserFullName,
                        invitation.Result.SentDate
                    )
                );
            }
        }

        var model = new
        {
            HideHeaderSignInLink = true
        };

        return View(ControllerConstants.ReportTrainingProviderViewName, model);
    }
}