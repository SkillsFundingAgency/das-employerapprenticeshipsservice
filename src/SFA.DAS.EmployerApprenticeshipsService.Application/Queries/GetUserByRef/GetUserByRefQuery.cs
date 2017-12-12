using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserByRef
{
    public class GetUserByRefQuery : IAsyncRequest<GetUserByRefResponse>
    {
        public string UserRef { get; set; }
    }
}
