using System;
using SFA.DAS.EAS.TestCommon.MockModels;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.TestCommon.ObjectMothers
{
    public class MessageObjectMother
    {
        public static  Message<T> Create<T>(T payload)
        {
            return new MockMessage<T>(payload, null, null);
        }

        public static Message<T> Create<T>(T payload, Action onCompleted, Action onAborted)
        {
            return new MockMessage<T>(payload, onCompleted, onAborted);
        }
    }
}
