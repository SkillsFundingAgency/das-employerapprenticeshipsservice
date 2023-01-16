using System.Threading;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesQueryHandler : IRequestHandler<GetAccountPayeSchemesQuery, GetAccountPayeSchemesResponse>
{
    private readonly IValidator<GetAccountPayeSchemesQuery> _validator;
    private IPayeSchemesService _payeSchemesService;

    public GetAccountPayeSchemesQueryHandler(
        IPayeSchemesService payeSchemesService,
        IValidator<GetAccountPayeSchemesQuery> validator )
    {
        _validator = validator;
        _payeSchemesService = payeSchemesService;
    }

    public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesQuery message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var payeSchemes =
            (await _payeSchemesService
                .GetPayeSchemes(message.HashedAccountId))
            .ToList();

        return new GetAccountPayeSchemesResponse
        {
            PayeSchemes = payeSchemes
        };
    }
}