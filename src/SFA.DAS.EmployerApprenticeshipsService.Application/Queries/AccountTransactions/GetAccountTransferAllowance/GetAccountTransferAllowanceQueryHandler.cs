using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Extensions;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransferAllowance
{
    public class GetAccountTransferAllowanceQueryHandler : IAsyncRequestHandler<GetAccountTransferAllowanceRequest, GetAccountTransferAllowanceResponse>
    {
        private readonly EmployerFinancialDbContext _db;
        private readonly IValidator<GetAccountTransferAllowanceRequest> _validator;
        private readonly LevyDeclarationProviderConfiguration _configuration;

        public GetAccountTransferAllowanceQueryHandler(EmployerFinancialDbContext db, IValidator<GetAccountTransferAllowanceRequest> validator, LevyDeclarationProviderConfiguration configuration)
        {
            _db = db;
            _validator = validator;
            _configuration = configuration;
        }

        public async Task<GetAccountTransferAllowanceResponse> Handle(GetAccountTransferAllowanceRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var transferAllowance = await _db.GetTransferAllowance(message.AccountId, _configuration.TransferAllowancePercentage);

            return new GetAccountTransferAllowanceResponse {TransferAllowance = transferAllowance };
        }
    }
}