using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountsByHashedId
{
    public class GetEmployerAccountsByHashedIdQuery : IAsyncRequest<GetEmployerAccountsByHashedIdResponse>
    {
        public string HashedAccountId { get; set; }
    }
}