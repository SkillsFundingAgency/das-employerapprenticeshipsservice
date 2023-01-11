using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    public class TestAppBuilder : IAppBuilder
    {
        public List<object> MiddlewareOptions { get; set; } = new List<object>();

        public IDictionary<string, object> Properties => new Dictionary<string, object>();       

        public object Build(Type returnType)
        {
            return null;
        }

        public IAppBuilder New()
        {   
            return new TestAppBuilder();
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            foreach(object i in args)
            {
                MiddlewareOptions.Add(i);
            }
            return this;
        }
    }
}
