using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef
{
    public class GetPayeSchemeByRefQuery : IAsyncRequest<GetPayeSchemeByRefResponse>
    {
        public string Ref { get; set; }
    }
}