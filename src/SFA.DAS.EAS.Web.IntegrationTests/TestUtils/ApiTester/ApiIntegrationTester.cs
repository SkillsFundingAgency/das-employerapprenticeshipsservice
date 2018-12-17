using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin.Testing;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Owin;
using SFA.DAS.EAS.Account.Api;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.NLog.Logger;
using StructureMap;
using WebApi.StructureMap;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    sealed class ApiIntegrationTester : IDisposable
    {
        private IntegrationTestDependencyResolver _dependencyResolver;
        private IntegrationTestExceptionLogger _exceptionLogger;
        private readonly Func<IContainer> _testSetupContainerInitialiser;
        private bool IsTestServerStarted => TestServer != null;
        private TestServer TestServer { get; set; }

        /// <summary>
        ///     Use this constructor for simple tests which do not require any data set up. If your test requires 
        ///     data set up then use the constructor that accepts a delegate for obtaining a test set up container.
        /// </summary>
        public ApiIntegrationTester() : this(null)
        {
        }

        /// <summary>
        ///     Constructor that takes a delegate that returns an IoC container to be used
        ///     for setting up the test. 
        /// </summary>
        public ApiIntegrationTester(Func<IContainer> testSetupContainer)
        {
            _testSetupContainerInitialiser = testSetupContainer;
        }

        /// <summary>
        ///     Send a GET to the specified URI using a test server and configuration created just for this call.
        /// </summary>
        /// <param name="call">Details the requirements of the call to be made.</param>
        public static async Task<CallResponse> InvokeIsolatedGetAsync(CallRequirements call)
        {
            using (var tester = new ApiIntegrationTester())
            {
                return await tester.InvokeGetAsync(call);
            }
        }

        public static async Task<CallResponse<TResult>> InvokeIsolatedGetAsync<TResult>(CallRequirements call)
        {
            using (var tester = new ApiIntegrationTester(TestSetupIoC.CreateIoC))
            {
                return await tester.InvokeGetAsync<TResult>(call);
            }
        }

        public Task<CallResponse> InvokeGetAsync(CallRequirements call)
        {
            return GetResponseAsync(call);
        }

        public Task<CallResponse<TResult>> InvokeGetAsync<TResult>(CallRequirements call)
        {
            EnsureStarted();

            return GetResponseAsync<TResult>(call);
        }

        /// <summary>
        ///     Call this when you want to set up some data prior to running the actual test.
        ///     A new container will be created and a new transaction will be started. 
        /// </summary>
        /// <typeparam name="TDbBuilder"></typeparam>
        /// <param name="initialiseAction"></param>
        /// <returns></returns>
        public async Task InitialiseData<TDbBuilder>(Func<TDbBuilder, Task> initialiseAction) where TDbBuilder : IDbBuilder
        {
            Contract.Requires(_testSetupContainerInitialiser != null, "Cannot initialise data without an IoC container - please use the constructor that accepts a delegate that returns a test setup container");
            
            var container = _testSetupContainerInitialiser();

            var builder = container.GetInstance<TDbBuilder>();

            builder.BeginTransaction();
            try
            {
                await initialiseAction(builder);
                builder.CommitTransaction();
            }
            catch (Exception)
            {
                builder.RollbackTransaction();
                throw;
            }
        }

        public void Dispose()
        {
            if (IsTestServerStarted)
            {
                EndTests();
            }
        }

        private void BeginTests()
        {
            TestServer = Microsoft.Owin.Testing.TestServer.Create(InitialiseHost);
        }

        private void EndTests()
        {
            TestServer?.Dispose();
            TestServer = null;
        }

        
        private async Task<CallResponse> GetResponseAsync(CallRequirements call)
        {
            var callResponse = new CallResponse();

            await FetchInitialResponseAsync(call, callResponse);

            return callResponse;
        }

        private async Task<CallResponse<TResult>> GetResponseAsync<TResult>(CallRequirements call)
        {
            var callResponse = new CallResponse<TResult>();

            await FetchInitialResponseAsync(call, callResponse);
            await FetchCompleteResponseAsync(callResponse);

            return callResponse;
        }

        private async Task FetchInitialResponseAsync(CallRequirements call, CallResponse response)
        {
            EnsureStarted();
            ClearDownForNewTest();
            await MakeCall(call, response);
            SaveCallDetails(response);
        }

        private async Task MakeCall(CallRequirements call, CallResponse response)
        {
            var client = GetClient(call);
            response.Response = await client.GetAsync(call.Uri);
        }

        private async Task FetchCompleteResponseAsync<TResult>(CallResponse<TResult> response)
        {
            var responseContent = await response.Response.Content.ReadAsStringAsync();
            response.Data = JsonConvert.DeserializeObject<TResult>(responseContent);
        }

        private HttpClient GetClient(CallRequirements call)
        {
            var client = TestServer.HttpClient;
            client.Timeout = call.TimeOut;
            return client;
        }

        private void ClearDownForNewTest()
        {
            _exceptionLogger.ClearException();
            _dependencyResolver.ClearCreationLog();
        }

        private void EnsureStarted()
        {
            if (!IsTestServerStarted)
            {
                BeginTests();
            }
        }

        private void InitialiseHost(IAppBuilder app)
        {
            // TODO: this ties this class to the EAS project - it would be better to inject this into the test harness
            var config = new HttpConfiguration();

            WebApiConfig.Register(config);
            CustomiseConfig(config);
            app.UseWebApi(config);
        }

        private void CustomiseConfig(HttpConfiguration config)
        {
            CustomiseIoC(config);
            CustomiseDependencyResolver(config);
            CustomiseExceptionLogger(config);
            CustomiseAssemblyResolver(config);
        }


        private void CustomiseIoC(HttpConfiguration config)
        {
            var container = config.DependencyResolver.GetService<IContainer>();
            container.Configure(c =>
            {
                c.For<ILoggingContext>().Use(Mock.Of<ILoggingContext>());
            });
        }

        private void CustomiseDependencyResolver(HttpConfiguration config)
        {
            // This allows us to track all IoC resolves so we can check the controllers that are created
            var container = config.DependencyResolver.GetService<IContainer>();
            _dependencyResolver = new IntegrationTestDependencyResolver(container);
            config.DependencyResolver = _dependencyResolver;
        }

        private void CustomiseExceptionLogger(HttpConfiguration config)
        {
            // This allows us to check any unhandled exceptions that get thrown by the controller
            _exceptionLogger = new IntegrationTestExceptionLogger();
            config.Services.Add(typeof(IExceptionLogger), _exceptionLogger);
        }

        private void CustomiseAssemblyResolver(HttpConfiguration config)
        {
            // This allows us to point the owin test server at the web project to find the controllers and routes.
            var assembliesResolver = new TestWebApiResolver<LegalEntitiesController>();
            config.Services.Replace(typeof(IAssembliesResolver), assembliesResolver);
        }

        private void SaveCallDetails(CallResponse callResponse)
        {
            callResponse.CreatedControllerTypes.AddRange(GetCreatedControllers());
            callResponse.UnhandledException = _exceptionLogger.Exception;
        }

        private IEnumerable<Type> GetCreatedControllers()
        {
            return _dependencyResolver
                .CreatedServiceTypes
                .Where(t => typeof(ApiController).IsAssignableFrom(t));
        }
    }
}