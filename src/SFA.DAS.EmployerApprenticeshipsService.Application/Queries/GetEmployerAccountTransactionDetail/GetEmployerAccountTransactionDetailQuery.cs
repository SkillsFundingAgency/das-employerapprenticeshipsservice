using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail
{
    public class GetEmployerAccountTransactionDetailQuery : IAsyncRequest<GetEmployerAccountTransactionDetailResponse>
    {
        public int Id { get; set; }
        public string HashedId { get; set; }
        public string ExternalUserId { get; set; }
    }
}
