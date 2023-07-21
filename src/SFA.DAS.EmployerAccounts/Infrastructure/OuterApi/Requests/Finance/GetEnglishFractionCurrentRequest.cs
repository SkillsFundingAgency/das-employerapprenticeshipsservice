using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Finance;

public class GetEnglishFractionCurrentRequest : IGetApiRequest
{
    private readonly string _hashedAccountId;
    private readonly UriBuilder _baseUri;

    public string GetUrl => $"accounts/{_hashedAccountId}/levy/english-fraction-current{_baseUri.Uri.Query}";

    public GetEnglishFractionCurrentRequest(string hashedAccountId, string[] empRefs)
    {
        _hashedAccountId = hashedAccountId;
        _baseUri = new UriBuilder();

        foreach (var empRef in empRefs)
        {
            if (!string.IsNullOrEmpty(_baseUri.Query))
                _baseUri.Query = _baseUri.Query.Substring(1) + $"&empRefs={empRef}";
            else
                _baseUri.Query = $"empRefs={empRef}";
        }
    }
}