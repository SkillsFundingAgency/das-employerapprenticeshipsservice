using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove
{
    public class GetAccountEmployerAgreementsRemoveRequest : IAsyncRequest<GetAccountEmployerAgreementsRemoveResponse>
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
    }
}
