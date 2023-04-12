using System.Threading;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.EmployerAccounts.Commands.ReportTrainingProvider;

public class ReportTrainingProviderCommandHandler : IRequestHandler<ReportTrainingProviderCommand>
{
    private const string ReportTrainingProviderTemplateId = "ReportTrainingProviderNotification";
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly IMessageSession _publisher;
    private readonly ILogger<ReportTrainingProviderCommandHandler> _logger;

    public ReportTrainingProviderCommandHandler(
        IMessageSession publisher,
        ILogger<ReportTrainingProviderCommandHandler> logger,
        EmployerAccountsConfiguration configuration)
    {
        _publisher = publisher;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(ReportTrainingProviderCommand message, CancellationToken cancellationToken)
    {
        var tokens = new Dictionary<string, string>()
        {
            {"employer_email", message.EmployerEmailAddress },
            {"email_reported_date", message.EmailReportedDate.ToString("g") },
            {"training_provider", message.TrainingProvider },
            {"training_provider_name", message.TrainingProviderName },
            {"invitation_email_sent_date", message.InvitationEmailSentDate.ToString("g") },
        };

        if (string.IsNullOrWhiteSpace(_configuration.ReportTrainingProviderEmailAddress))
        {
            var exception = new InvalidConfigurationValueException(nameof(_configuration.ReportTrainingProviderEmailAddress));
            _logger.LogError(exception, "Report Training Provider Email must be provided in configuration");
            throw exception;
        }

        await _publisher.Send(new SendEmailCommand(ReportTrainingProviderTemplateId, _configuration.ReportTrainingProviderEmailAddress, tokens));

        return Unit.Value;
    }
}