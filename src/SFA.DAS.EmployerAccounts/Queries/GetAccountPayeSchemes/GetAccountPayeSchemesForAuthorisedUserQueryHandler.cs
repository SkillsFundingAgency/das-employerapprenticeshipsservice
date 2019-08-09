using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesForAuthorisedUserQueryHandler : IAsyncRequestHandler<GetAccountPayeSchemesForAuthorisedUserQuery, GetAccountPayeSchemesResponse>
    {
        private readonly IValidator<GetAccountPayeSchemesForAuthorisedUserQuery> _validator;
        private readonly IMediator _mediator;


        public GetAccountPayeSchemesForAuthorisedUserQueryHandler(
            IValidator<GetAccountPayeSchemesForAuthorisedUserQuery> validator, IMediator mediator)
        {
            _validator = validator;
            _mediator = mediator;
        }

        public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesForAuthorisedUserQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            return await _mediator.SendAsync(
                new GetAccountPayeSchemesQuery
                {
                    HashedAccountId = message.HashedAccountId
                });
        }
    }
}