using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsQuery : IAsyncRequest<GetEmployerAccountTransactionsResponse>
    {
        public int AccountId { get; set; }
    }
}
