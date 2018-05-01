using System;
using System.Threading.Tasks;
using FluentAssertions.Specialized;

namespace SFA.DAS.EAS.TestCommon
{
    public abstract class FluentTest<T> where T : FluentTestFixture, new()
    {
        public void Run(Action<T> assert)
        {
            Run(null, null, assert);
        }

        public void Run(Action<T> act, Action<T> assert)
        {
            Run(null, act, assert);
        }

        public void Run(Action<T> arrange, Action<T> act, Action<T> assert)
        {
            var testFixture = new T();
            
            arrange?.Invoke(testFixture);
            act?.Invoke(testFixture);
            assert?.Invoke(testFixture);
        }

        public void Run<TException>(Func<T, Action, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            Run(null, null, assert);
        }

        public void Run<TException>(Action<T> act, Func<T, Action, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            Run(null, act, assert);
        }

        public void Run<TException>(Action<T> arrange, Action<T> act, Func<T, Action, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            var testFixture = new T();

            arrange?.Invoke(testFixture);
            assert(testFixture, () => act?.Invoke(testFixture));
        }

        public Task RunAsync(Action<T> assert)
        {
            return RunAsync(null, null, assert);
        }

        public Task RunAsync(Func<T, Task> act, Action<T> assert)
        {
            return RunAsync(null, act, assert);
        }

        public async Task RunAsync(Action<T> arrange, Func<T, Task> act, Action<T> assert)
        {
            var testFixture = new T();

            arrange?.Invoke(testFixture);

            if (act != null)
            {
                await act(testFixture);
            }

            assert?.Invoke(testFixture);
        }

        public Task RunAsync<TException>(Func<T, Func<Task>, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            return RunAsync(null, null, assert);
        }

        public Task RunAsync<TException>(Func<T, Task> act, Func<T, Func<Task>, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            return RunAsync(null, act, assert);
        }

        public Task RunAsync<TException>(Action<T> arrange, Func<T, Task> act, Func<T, Func<Task>, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            var testFixture = new T();

            arrange?.Invoke(testFixture);

            assert(testFixture, async () =>
            {
                if (act != null)
                {
                    await act(testFixture);
                }
            });

            return Task.CompletedTask;
        }
    }
}