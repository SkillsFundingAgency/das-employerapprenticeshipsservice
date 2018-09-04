using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetPayeSchemeByRef
{
    public class GetPayeSchemeByRefQuery : IAsyncRequest<GetPayeSchemeByRefResponse>
    {
        public string HashedAccountId { get; set; }
        public string Ref { get; set; }
    }
}