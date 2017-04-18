using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetLevyDeclarationsByAccountAndPeriod
{
    public class GetLevyDeclarationsByAccountAndPeriodQueryHandler : IAsyncRequestHandler<GetLevyDeclarationsByAccountAndPeriodRequest, GetLevyDeclarationsByAccountAndPeriodResponse>
    {
        private readonly IDasLevyRepository _repository;
        private readonly IHashingService _hashingService;

        public GetLevyDeclarationsByAccountAndPeriodQueryHandler(IDasLevyRepository repository, IHashingService hashingService)
        {
            _repository = repository;
            _hashingService = hashingService;
        }

        public async Task<GetLevyDeclarationsByAccountAndPeriodResponse> Handle(GetLevyDeclarationsByAccountAndPeriodRequest message)
        {
            var accountId = GetAccountId(message.HashedAccountId);
            var declarations = await _repository.GetAccountLevyDeclarations(accountId, message.PayrollYear, message.PayrollMonth);
            return new GetLevyDeclarationsByAccountAndPeriodResponse { Declarations = declarations };
        }

        private long GetAccountId(string hashedAccountId)
        {
            return _hashingService.DecodeValue(hashedAccountId);
        }
    }
}