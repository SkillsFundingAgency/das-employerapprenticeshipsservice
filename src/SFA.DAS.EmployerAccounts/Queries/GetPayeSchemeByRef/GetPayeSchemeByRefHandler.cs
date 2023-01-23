using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;

public class GetPayeSchemeByRefHandler : IRequestHandler<GetPayeSchemeByRefQuery, GetPayeSchemeByRefResponse>
{
    private readonly IValidator<GetPayeSchemeByRefQuery> _validator;
    private readonly IPayeRepository _payeRepository;

    public GetPayeSchemeByRefHandler(IValidator<GetPayeSchemeByRefQuery> validator, IPayeRepository payeRepository)
    {
        _validator = validator;
        _payeRepository = payeRepository;
    }

    public async Task<GetPayeSchemeByRefResponse> Handle(GetPayeSchemeByRefQuery message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var payeScheme = await _payeRepository.GetPayeForAccountByRef(message.HashedAccountId, message.Ref);

        return new GetPayeSchemeByRefResponse { PayeScheme = payeScheme};
    }
}