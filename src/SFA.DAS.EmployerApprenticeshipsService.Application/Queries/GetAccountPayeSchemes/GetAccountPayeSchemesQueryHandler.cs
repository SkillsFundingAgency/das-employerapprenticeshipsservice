using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesQueryHandler : IAsyncRequestHandler<GetAccountPayeSchemesRequest, GetAccountPayeSchemesResponse>
    {
        private readonly IAccountRepository _accountRepository;

        //TODO needs validator. And tests. make sure you cant access someone elses schemes!

        public GetAccountPayeSchemesQueryHandler(IAccountRepository accountRepository)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            _accountRepository = accountRepository;
        }

        public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesRequest message)
        {
            var payeSchemes = await _accountRepository.GetPayeSchemesByHashedId(message.HashedId);

            return new GetAccountPayeSchemesResponse
            {
                PayeSchemes = payeSchemes
            };
        }
    }
}