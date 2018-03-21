using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.TestCommon
{
    public abstract class FluentTest<T> where T : new()
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
    }

    public abstract class FluentTestAsync<T> where T : new()
    {
        public Task Run(Func<T, Task> act, Action<T> assert)
        {
            return Run(null, act, assert);
        }

        public async Task Run(Action<T> arrange, Func<T, Task> act, Action<T> assert)
        {
            var testFixture = new T();
            
            arrange?.Invoke(testFixture);

            if (act != null)
            {
                await act(testFixture);
            }

            assert?.Invoke(testFixture);
        }
    }
}