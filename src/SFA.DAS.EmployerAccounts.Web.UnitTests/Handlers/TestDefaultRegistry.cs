using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.Results;
using SFA.DAS.EmployerAccounts.Web.Authorization;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Handlers
{
    public class TestDefaultRegistry : Registry
    {
       

        public TestDefaultRegistry()
        {

            For<IDefaultAuthorizationHandler>().Use<Authorization.DefaultAuthorizationHandler>();
            For<IDefaultAuthorizationHandler>().Use<TestDefaultAuthorizationHandler>();
            //For<IAuthorizationContextProvider>().Use<TestAuthorizationContextProvider>();
            //For<IAuthorizationContextProvider>().Use<TestImpersonationAuthorizationContext>();

            var authorizationContextProvider = For<IAuthorizationContextProvider>().Use<TestAuthorizationContextProvider>();
            For<IAuthorizationContextProvider>().Use<TestImpersonationAuthorizationContext>()
           .Ctor<IAuthorizationContextProvider>().Is(authorizationContextProvider);

        }

    }

    public class TestDefaultAuthorizationHandler : IDefaultAuthorizationHandler
    {
        public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
        {
            throw new NotImplementedException();
            //return  Task.FromResult(new AuthorizationResult());
        }
    }

    public class TestAuthorizationContextProvider : IAuthorizationContextProvider
    {
        public IAuthorizationContext GetAuthorizationContext()
        {
            throw new NotImplementedException();
        }
    }

    public class TestImpersonationAuthorizationContext : IAuthorizationContextProvider
    {
        public IAuthorizationContext GetAuthorizationContext()
        {
            throw new NotImplementedException();
        }
    }

    public class DefaultRegistryResolved 
    {
        public IContainer Container { get; set; }
        

        [SetUp]
        public void Arrange()
        {
            Container = new Container(c =>
            {
                c.AddRegistry<EmployerAccounts.Web.DependencyResolution.DefaultRegistry>();
                c.AddRegistry<TestDefaultRegistry>();
            });
        }

        [Test]
        public void ThenTheAuditApiClientForSupportUserTypeIsReturned()
        {
            //Act
            var instance = Container.GetInstance<IDefaultAuthorizationHandler>();

            //Assert
            Assert.IsInstanceOf<TestDefaultAuthorizationHandler>(instance);
        }

        //[Test]
        //public void ThenTheAuditApiClientForSupportUserTypeIsReturned1()
        //{
        //    //Act
        //    var instance = Container.GetInstance<IDefaultAuthorizationHandler>();

        //    //Assert
        //    Assert.IsInstanceOf<Authorization.DefaultAuthorizationHandler>(instance);
        //}

        [Test]
        public void ThenTheAuditApiClientForSupportUserTypeIsReturned2()
        {
            //Act
            var instance = Container.GetInstance<IAuthorizationContextProvider>();

            //Assert
            Assert.IsInstanceOf<TestImpersonationAuthorizationContext>(instance);
        }


        //[Test]
        //public void ThenTheAuditApiClientForSupportUserTypeIsReturned3()
        //{
        //    //Act
        //    var instance = Container.GetInstance<IAuthorizationContextProvider>();

        //    //Assert
        //    Assert.IsInstanceOf<TestAuthorizationContextProvider>(instance);
        //}

    }


}
