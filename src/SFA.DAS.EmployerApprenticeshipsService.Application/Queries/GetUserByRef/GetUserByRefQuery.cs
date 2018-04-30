using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetUserByRef
{
    public class GetUserByRefQuery : IAsyncRequest<GetUserByRefResponse>
    {
        public Guid ExternalUserId { get; set; }
    }
}
