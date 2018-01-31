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
using SFA.DAS.EAS.Api;
using SFA.DAS.EAS.Api.Controllers;
using SFA.DAS.EAS.Api.DependencyResolution;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils
{
    sealed class ApiIntegrationTester : IDisposable
    {
        private IntegrationTestDependencyResolver _dependencyResolver;

        private IntegrationTestExceptionHandler _exceptionHandler;

        public bool IsTestServerStarted => TestServer != null;

        public TestServer TestServer { get; private set; }

        /// <summary>
        ///     An overload to <see cref="InvokeIsolatedGetAsync"/> that does not return a value (only a status code).
        /// </summary>
        /// <param name="call">Details the requirements of teh call to be made.</param>
        /// <remarks>
        ///     This is a shortcut to run individual tests. If running multiple tests then using <see cref="InvokeGetAsync"/>
        ///     is more efficient.
        /// </remarks>
        public static async Task InvokeIsolatedGetAsync(CallRequirements call)
        {
            using (var tester = new ApiIntegrationTester())
            {
                await tester.InvokeGetAsync(call);
            }
        }

        /// <summary>
        ///     Send a GET to the specified URI using a test server and configuration created just for this call.
        /// </summary>
        /// <typeparam name="TResult">The type of the result expected from the GET request.</typeparam>
        /// <param name="call">Details the requirements of the call to be made.</param>
        /// <returns>A task that will result in an instance of <see cref="TResult"/>.</returns>
        public static async Task<TResult> InvokeIsolatedGetAsync<TResult>(CallRequirements<TResult> call)
        {
            using (var tester = new ApiIntegrationTester() )
            {
                return await tester.InvokeGetAsync(call);
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

        public Task InvokeGetAsync(CallRequirements call)
        {
            return GetResponseAsync(call);
        }

        public async Task<TResult> InvokeGetAsync<TResult>(CallRequirements<TResult> call)
        {
            var result = await GetResponseAsync(call);
            call.Result = await FetchCompleteResponse<TResult>(result);
            return call.Result;
        }

        private async Task<HttpResponseMessage> GetResponseAsync(CallRequirements call)
        {
            EnsureStarted();
            ClearDownForNewTest();
            var result = await GetClient(call).GetAsync(call.Uri);
            CheckCallWasSuccessful(result, call);
            return result;
        }

        private HttpClient GetClient(CallRequirements call)
        {
            var client = TestServer.HttpClient;
            client.Timeout = call.TimeOut;
            return client;
        }

        private void ClearDownForNewTest()
        {
            _exceptionHandler.ClearException();
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
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            CustomiseIoC(config);
            app.UseWebApi(config);
        }

        private void CustomiseIoC(HttpConfiguration configuration)
        {
            var existingExceptionHandler = (IExceptionHandler) configuration.Services.GetService(typeof(IExceptionHandler));
            _exceptionHandler = new IntegrationTestExceptionHandler(existingExceptionHandler);

            configuration.Services.Replace(typeof(IAssembliesResolver), new TestWebApiResolver<AccountLegalEntitiesController>());
            configuration.Services.Replace(typeof(IExceptionHandler), _exceptionHandler);

            StructuremapMvc.Start();
            StructuremapWebApi.Start();
            var container = ((StructureMapWebApiDependencyResolver)GlobalConfiguration.Configuration.DependencyResolver).Container;
            var requestContextMock = new Mock<IRequestContext>();
            container.Inject(requestContextMock.Object);
            _dependencyResolver = new IntegrationTestDependencyResolver(container);
            configuration.DependencyResolver = _dependencyResolver;
        }

        private void CheckCallWasSuccessful(HttpResponseMessage response, CallRequirements call)
        {
            StringBuilder failMessage = new StringBuilder();

            if (!IsAcceptableStatusCode(response, call.AcceptableStatusCodes))
            {
                failMessage.AppendLine($"Received response {response.StatusCode} " +
                                       $"when expected any of [{string.Join(",", call.AcceptableStatusCodes)}]. " +
                                       $"Additional information sent to the client: {response.ReasonPhrase}. ");
            }

            if (WasUnacceptableExceptionThrownInServer(call))
            {
                failMessage.AppendLine($"An unexpected unhandled exception occurred in the server during the call:{_exceptionHandler.Exception.GetType().Name} - {_exceptionHandler.Exception.Message}");
            }

            if (!WasExpectedControllerCreated(call.ExpectedControllerType))
            {
                failMessage.AppendLine($"The controller {call.ExpectedControllerType.Name} was not created by DI. "+
                                       $"Controllers that were created are: {string.Join(",", GetCreatedControllers())}");
            }

            if (failMessage.Length > 0)
            {
                Assert.Fail(failMessage.ToString());
            }
        }

        private bool WasUnacceptableExceptionThrownInServer(CallRequirements call)
        {
            return _exceptionHandler.IsFaulted
                   && (call.IgnoreExceptionTypes == null 
                        || !call.IgnoreExceptionTypes.Contains(_exceptionHandler.Exception.GetType()));
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

        private bool IsAcceptableStatusCode(HttpResponseMessage response, IEnumerable<HttpStatusCode> acceptableStatusCodes)
        {
            return acceptableStatusCodes.Contains(response.StatusCode);
        }

        private async Task<TResult> FetchCompleteResponse<TResult>(HttpResponseMessage message)
        {
            var responseContent = await message.Content.ReadAsStringAsync();
            var returnedObject = JsonConvert.DeserializeObject<TResult>(responseContent);
            return returnedObject;
        }
    }
}