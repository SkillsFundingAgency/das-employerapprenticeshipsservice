using Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    public class TestAppBuilder : IAppBuilder
    {
        public IList<Tuple<object, object[]>> BuildMiddleware;

        public TestAppBuilder()
        {   
            BuildMiddleware = new List<Tuple<object, object[]>>();
        }

        public IDictionary<string, object> Properties => new Dictionary<string, object>();       

        public object Build(Type returnType)
        {
            return new Func<IDictionary<string, object>, Task>(new NotFound().Invoke);            
        }

        public IAppBuilder New()
        {   
            return new TestAppBuilder();
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            BuildMiddleware.Add(ToMiddlewareObjectFactory(middleware, args));
            return this;
        }
       
        private static Tuple<object, object[]> ToMiddlewareObjectFactory(object middlewareObject, object[] args)
        {
            object middlewareDelegate = middlewareObject as object;
            return Tuple.Create(middlewareDelegate, args);
        }
    }

    internal class NotFound
    {
        private static readonly Task Completed = CreateCompletedTask();

        private static Task CreateCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        public Task Invoke(IDictionary<string, object> env)
        {
            env["owin.ResponseStatusCode"] = 404;
            return Completed;
        }
    }

}
