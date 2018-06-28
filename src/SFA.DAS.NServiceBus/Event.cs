using System;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public class Event : IEvent
    {
        public DateTime Created { get; set; }
    }
}