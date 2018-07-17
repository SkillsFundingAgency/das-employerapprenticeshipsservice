using System;
using System.Collections.Generic;
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
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;
using SFA.DAS.EAS.Account.Api;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.NLog.Logger;
using StructureMap;
using WebApi.StructureMap;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    sealed class ApiIntegrationTester : IDisposable
    {
        private IntegrationTestDependencyResolver _dependencyResolver;

        private IntegrationTestExceptionLogger _exceptionLogger;

        private readonly Lazy<DbBuilder> _dbBuilder;

        public bool IsTestServerStarted => TestServer != null;

        public ApiIntegrationTester()
        {
            _dbBuilder = new Lazy<DbBuilder>(Resolve<DbBuilder>);
        }

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
            using (var tester = new ApiIntegrationTester())
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
            return GetResponseAsync<TResult>(call);
        }

        /// <summary>
        ///     Use the DI container used for the test to resolve the specified service.
        /// </summary>
        public T Resolve<T>()
        {
            EnsureStarted();

            return _dependencyResolver.Container.GetInstance<T>();
        }

        public DbBuilder DbBuilder => _dbBuilder.Value;

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
            var config = new HttpConfiguration();

            WebApiConfig.Register(config);
            CustomiseConfig(config);
            app.UseWebApi(config);
        }

        private void CustomiseConfig(HttpConfiguration config)
        {
            var container = config.DependencyResolver.GetService<IContainer>();
            var assembliesResolver = new TestWebApiResolver<LegalEntitiesController>();

            container.Configure(c =>
            {
                c.For<ILoggingContext>().Use(Mock.Of<ILoggingContext>());
                c.For<IPublicHashingService>().Use(Mock.Of<IPublicHashingService>());
            });

            _dependencyResolver = new IntegrationTestDependencyResolver(container);
            _exceptionLogger = new IntegrationTestExceptionLogger();

            config.DependencyResolver = _dependencyResolver;
            config.Services.Add(typeof(IExceptionLogger), _exceptionLogger);
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