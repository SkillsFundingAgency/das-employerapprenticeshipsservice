using System;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiRequests.Finance
{
    public class GetEnglishFractionCurrentRequest : IGetApiRequest
    {
        private readonly string _hashedAccountId;
        private readonly UriBuilder _baseUri;

        public string GetUrl => _baseUri.Uri.PathAndQuery;

        public GetEnglishFractionCurrentRequest(string hashedAccountId, string[] empRefs)
        {
            _hashedAccountId = hashedAccountId;
            _baseUri = new UriBuilder
            {
                Path = $"accounts/{_hashedAccountId}/levy/english-fraction-current"
            };

            foreach (string empRef in empRefs)
            {
                if (!string.IsNullOrEmpty(_baseUri.Query))
                    _baseUri.Query = _baseUri.Query.Substring(1) + $"&empRefs={empRef}";
                else
                    _baseUri.Query = $"empRefs={empRef}";
            }
        }
    }
}
