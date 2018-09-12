using System;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EmployerAccounts.Models
{
    public class HealthCheck : Entity
    {
        public virtual int Id { get; protected set; }
        public virtual DateTime PublishedEvent { get; protected set; }
        public virtual DateTime? ReceivedEvent { get; protected set; }

        public void PublishEvent()
        {
            PublishedEvent = DateTime.UtcNow;

            Publish<HealthCheckEvent>(e =>
            {
                e.Id = Id;
                e.Created = PublishedEvent;
            });
        }

        public void ReceiveEvent()
        {
            ReceivedEvent = DateTime.UtcNow;
        }
    }
}