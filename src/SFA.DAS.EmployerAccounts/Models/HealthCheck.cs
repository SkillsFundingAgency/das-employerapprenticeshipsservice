using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EmployerAccounts.Models
{
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

        public void PublishEvent()
        {
            PublishedEvent = DateTime.UtcNow;

            Publish<HealthCheckEvent>(e =>
            {
                e.Id = Id;
                e.Created = PublishedEvent;
            });
        }

        public async Task SendRequest(Func<Task> run)
        {
            SentRequest = DateTime.UtcNow;

            await run();

            ReceivedResponse = DateTime.UtcNow;
        }

        public void ReceiveEvent(HealthCheckEvent message)
        {
            ReceivedEvent = DateTime.UtcNow;
        }
    }
}