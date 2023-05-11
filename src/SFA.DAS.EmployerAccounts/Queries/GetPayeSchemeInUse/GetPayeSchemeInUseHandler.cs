using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;

public class GetPayeSchemeInUseHandler : IRequestHandler<GetPayeSchemeInUseQuery,GetPayeSchemeInUseResponse>
{
    private readonly IValidator<GetPayeSchemeInUseQuery> _validator;
    private readonly IEmployerSchemesRepository _employerSchemesRepository;

    public GetPayeSchemeInUseHandler(IValidator<GetPayeSchemeInUseQuery> validator, IEmployerSchemesRepository employerSchemesRepository)
    {
        _validator = validator;
        _employerSchemesRepository = employerSchemesRepository;
    }

    public async Task<GetPayeSchemeInUseResponse> Handle(GetPayeSchemeInUseQuery message, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var scheme = await _employerSchemesRepository.GetSchemeByRef(message.Empref);

        return new GetPayeSchemeInUseResponse {PayeScheme = scheme};
    }
}