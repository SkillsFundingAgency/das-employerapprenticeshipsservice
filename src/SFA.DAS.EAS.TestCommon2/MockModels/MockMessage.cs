using System;
using System.Threading.Tasks;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.TestCommon.MockModels
{
    public class MockMessage<T> : Message<T>
    {
        private readonly Action _onComplete;
        private readonly Action _onAborted;
        public bool Completed { get; protected set; }
        public bool Aborted { get; protected set; }

        public MockMessage(T obj, Action onComplete, Action onAborted) : base(obj)
        {
            _onComplete = onComplete;
            _onAborted = onAborted;
        }

        public override Task CompleteAsync()
        {
            Completed = true;
            _onComplete?.Invoke();

            return Task.Delay(0);
        }

        public override Task AbortAsync()
        {
            Aborted = true;
            _onAborted?.Invoke();

            return Task.Delay(0);
        }
    }
}
