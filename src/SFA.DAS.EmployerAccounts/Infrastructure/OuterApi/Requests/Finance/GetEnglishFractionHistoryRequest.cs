using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Finance;

public class GetEnglishFractionHistoryRequest : IGetApiRequest
{
    private readonly string _hashedAccountId;
    private readonly string _empRef;

    public string GetUrl => $"accounts/{_hashedAccountId}/levy/english-fraction-history?empRef={_empRef}";

    public GetEnglishFractionHistoryRequest(string hashedAccountId, string empRef)
    {
        _hashedAccountId = hashedAccountId;
        _empRef = empRef;
    }
}