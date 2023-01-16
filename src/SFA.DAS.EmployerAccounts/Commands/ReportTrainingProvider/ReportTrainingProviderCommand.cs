namespace SFA.DAS.EmployerAccounts.Commands.ReportTrainingProvider;

public class ReportTrainingProviderCommand : IRequest
{
    public string EmployerEmailAddress { get; }
    public DateTime EmailReportedDate { get; }
    public string TrainingProvider { get; }
    public string TrainingProviderName { get; }
    public DateTime InvitationEmailSentDate { get; }

    public ReportTrainingProviderCommand(string employerEmail, DateTime reportedDate, string trainingProvider, string trainingProviderName, DateTime sentDate)
    {
        EmployerEmailAddress = employerEmail;
        EmailReportedDate = reportedDate;
        TrainingProvider = trainingProvider;
        TrainingProviderName = trainingProviderName;
        InvitationEmailSentDate = sentDate;
    }
}