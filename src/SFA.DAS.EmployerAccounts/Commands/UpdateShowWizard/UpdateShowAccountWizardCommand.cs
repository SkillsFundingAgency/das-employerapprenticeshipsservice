namespace SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;

public class UpdateShowAccountWizardCommand : IAsyncRequest
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
    public bool ShowWizard { get; set; }
}