using System.Net.Http;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester
{
    public class CallResponse
    {
        public HttpResponseMessage Response { get; set; }
        public bool Failed { get; set; }
        public string FailureMessage { get; set; }
    }

    /// <summary>
    ///     A generic form of <see cref="CallResponse"/> which will deserialise the full response into the
    ///     <see cref="Data"/>.
    /// </summary>
    public class CallResponse<TResponse> : CallResponse
    {
        /// <summary>
        ///     The response returned from the call.
        /// </summary>
        public TResponse Data { get; set; }
    }
}