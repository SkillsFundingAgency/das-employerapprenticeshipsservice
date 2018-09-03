using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef
{
    /// <summary>
    ///  AML-2454: Copy to finance
    /// </summary>
    public class GetPayeSchemeByRefQuery : IAsyncRequest<GetPayeSchemeByRefResponse>
    {
        public string HashedAccountId { get; set; }
        public string Ref { get; set; }
    }
}