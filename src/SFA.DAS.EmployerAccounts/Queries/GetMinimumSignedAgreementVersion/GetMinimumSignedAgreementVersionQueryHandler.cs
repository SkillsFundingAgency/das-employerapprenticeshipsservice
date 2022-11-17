using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion
{
    public class GetMinimumSignedAgreementVersionQueryHandler : IAsyncRequestHandler<GetMinimumSignedAgreementVersionQuery, GetMinimumSignedAgreementVersionResponse>
    {
        private readonly IMediator _mediator;
        private readonly IValidator<GetMinimumSignedAgreementVersionQuery> _validator;
        private readonly IConfigurationProvider _configurationProvider;

        public GetMinimumSignedAgreementVersionQueryHandler(
            IMediator mediator,
            IValidator<GetMinimumSignedAgreementVersionQuery> validator,
            IConfigurationProvider configurationProvider)
        {
            _mediator = mediator;
            _validator = validator;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetMinimumSignedAgreementVersionResponse> Handle(GetMinimumSignedAgreementVersionQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var agreements = await _mediator.SendAsync(new GetEmployerAgreementsByAccountIdRequest { AccountId = message.AccountId }).ConfigureAwait(false);
            var minAgreementVersion = agreements.EmployerAgreements.Select(ea => ea.AccountLegalEntity.SignedAgreementVersion.GetValueOrDefault(0)).Min();

            return new GetMinimumSignedAgreementVersionResponse
            {
                MinimumSignedAgreementVersion = minAgreementVersion
            };
        }
    }
}