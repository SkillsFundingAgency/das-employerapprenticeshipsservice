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
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper;
using SFA.DAS.NLog.Logger;
using StructureMap;
using WebApi.StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester
{
    sealed class ApiIntegrationTester : IDisposable
    {
        private IntegrationTestDependencyResolver _dependencyResolver;

        private IntegrationTestExceptionLogger _exceptionLogger;

        private readonly Func<IContainer> _testSetupContainerInitialiser;

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

        public bool IsTestServerStarted => TestServer != null;

        public TestServer TestServer { get; private set; }

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

        public void BeginTests()
        {
            TestServer = TestServer.Create(InitialiseHost);
        }
        
        public void Dispose()
        {
            if (IsTestServerStarted)
            {
                EndTests();
            }
        }

        public void EndTests()
        {
            TestServer?.Dispose();
            TestServer = null;
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

        public T GetTransientInstance<T>()
        {
            EnsureStarted();

            return _dependencyResolver.Container.GetNestedContainer().GetInstance<T>();
        }

        public void InitialiseData<TDbBuilder>(Action<TDbBuilder> initialiseAction) where TDbBuilder : IDbBuilder
        {
            Contract.Requires(_testSetupContainerInitialiser != null, "Cannot initialise data without an IoC container - please use the constructor that accepts a delegate that returns a test setup container");
            
            var container = _testSetupContainerInitialiser();

            var builder = container.GetInstance<TDbBuilder>();

            builder.BeginTransaction();
            try
            {
                initialiseAction(builder);
                builder.CommitTransaction();
            }
            catch (Exception)
            {
                builder.RollbackTransaction();
                throw;
            }
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
            CheckCallWasSuccessful(call, response);
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

        private void CheckCallWasSuccessful(CallRequirements call, CallResponse response)
        {
            StringBuilder failMessage = new StringBuilder();

            CheckStatusCode(call, response, failMessage);

            CheckUnhandledExceptions(call, failMessage);

            CheckExpectedControllerCalled(call, failMessage);

            if (failMessage.Length > 0)
            {
                response.Failed = true;
                response.FailureMessage = failMessage.ToString();
                Assert.Fail(response.FailureMessage);
            }
        }

        private void CheckExpectedControllerCalled(CallRequirements call, StringBuilder failMessage)
        {
            if (!WasExpectedControllerCreated(call.ExpectedControllerType))
            {
                failMessage.AppendLine($"The controller {call.ExpectedControllerType.Name} was not created by DI. " +
                                       $"Controllers that were created are: {string.Join(",", GetCreatedControllers())}");
            }
        }

        private void CheckUnhandledExceptions(CallRequirements call, StringBuilder failMessage)
        {
            if (WasUnacceptableExceptionThrownInServer(call))
            {
                failMessage.AppendLine(
                    $"An unexpected unhandled exception occurred in the server during the call:{_exceptionLogger.Exception.GetType().Name} - {_exceptionLogger.Exception.Message}");
            }
        }

        private void CheckStatusCode(CallRequirements call, CallResponse response, StringBuilder failMessage)
        {
            if (!IsAcceptableStatusCode(response, call.AcceptableStatusCodes))
            {
                failMessage.AppendLine($"Received response {response.Response.StatusCode} " +
                                       $"when expected any of [{string.Join(",", call.AcceptableStatusCodes.Select(sc => sc))}]. " +
                                       $"Additional information sent to the client: {response.Response.ReasonPhrase}. ");
            }
        }

        private bool WasUnacceptableExceptionThrownInServer(CallRequirements call)
        {
            return _exceptionLogger.IsFaulted
                   && (call.IgnoreExceptionTypes == null
                        || !call.IgnoreExceptionTypes.Contains(_exceptionLogger.Exception.GetType()));
        }

        private bool WasExpectedControllerCreated(Type controllerType)
        {
            return controllerType == null || _dependencyResolver.WasServiceCreated(controllerType);
        }

        private IEnumerable<string> GetCreatedControllers()
        {
            return _dependencyResolver
                .CreatedServiceTypes
                .Where(t => typeof(ApiController).IsAssignableFrom(t))
                .Select(t => t.Name);
        }

        private bool IsAcceptableStatusCode(CallResponse response, IEnumerable<HttpStatusCode> acceptableStatusCodes)
        {
            return acceptableStatusCodes == null || acceptableStatusCodes.Contains(response.Response.StatusCode);
        }
    }
}