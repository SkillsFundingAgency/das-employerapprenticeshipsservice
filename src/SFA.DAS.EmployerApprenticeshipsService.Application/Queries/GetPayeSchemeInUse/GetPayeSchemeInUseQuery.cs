using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetPayeSchemeInUse
{
    public class GetPayeSchemeInUseQuery : IAsyncRequest<GetPayeSchemeInUseResponse>
    {
        public string Empref { get; set; }
    }
}
