namespace SFA.DAS.EmployerAccounts.Models;

public class HealthCheck : Entity
{
    public virtual int Id { get; protected set; }
    public virtual Guid UserRef { get; protected set; }
    public virtual DateTime SentRequest { get; protected set; }
    public virtual DateTime? ReceivedResponse { get; protected set; }
    public virtual DateTime PublishedEvent { get; protected set; }
    public virtual DateTime? ReceivedEvent { get; protected set; }

    public HealthCheck(Guid userRef)
    {
        UserRef = userRef;
    }

    protected HealthCheck()
    {
    }

    public async Task Run(Func<Task> request)
    {
        await SendRequest(request);
        PublishEvent();
    }

    public void ReceiveEvent(HealthCheckEvent message)
    {
        ReceivedEvent = DateTime.UtcNow;
    }

    private async Task SendRequest(Func<Task> run)
    {
        SentRequest = DateTime.UtcNow;

        try
        {
            await run();
            ReceivedResponse = DateTime.UtcNow;
        }
        catch
        {
            // comment to appease sonarcloud
        }
    }

    private void PublishEvent()
    {
        PublishedEvent = DateTime.UtcNow;

        Publish<HealthCheckEvent>(e =>
        {
            e.Id = Id;
            e.Created = PublishedEvent;
        });
    }
}