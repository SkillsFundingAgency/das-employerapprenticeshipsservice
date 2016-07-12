using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts
{
    public class GetUserAccountsQueryHandler : IAsyncRequestHandler<GetUserAccountsQuery, GetUserAccountsQueryResponse>
    {
        private readonly IUserAccountRepository _userAccountsRepository;

        public GetUserAccountsQueryHandler(IUserAccountRepository userAcountRepository)
        {
            _userAccountsRepository = userAcountRepository;
        }

        public async Task<GetUserAccountsQueryResponse> Handle(GetUserAccountsQuery message)
        {
            var userId = message.UserId;

            var accounts =  _userAccountsRepository.GetAccountsByUserId(userId);
            return new GetUserAccountsQueryResponse {Accounts = accounts};
        }
    }

    public class GetUserAccountsQueryResponse
    {
        public Accounts Accounts { get; set; }
    }
}