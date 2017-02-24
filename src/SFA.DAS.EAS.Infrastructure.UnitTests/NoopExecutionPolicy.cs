using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Infrastructure.UnitTests
{
    public class NoopExecutionPolicy : ExecutionPolicies.ExecutionPolicy
    {
        public override void Execute(Action action)
        {
        }
        public override async Task ExecuteAsync(Func<Task> action)
        {
            await action();
        }

        public override T Execute<T>(Func<T> func)
        {
            return func();
        }
        public override async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            return await func();
        }
    }
}
