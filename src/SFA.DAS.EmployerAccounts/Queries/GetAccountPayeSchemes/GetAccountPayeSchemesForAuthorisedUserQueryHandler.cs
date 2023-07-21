using System.Threading;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesForAuthorisedUserQueryHandler : IRequestHandler<GetAccountPayeSchemesForAuthorisedUserQuery, GetAccountPayeSchemesResponse>
{
    private readonly IPayeSchemesWithEnglishFractionService _payeSchemesService;
    private readonly IValidator<GetAccountPayeSchemesForAuthorisedUserQuery> _validator;

    public GetAccountPayeSchemesForAuthorisedUserQueryHandler(
        IPayeSchemesWithEnglishFractionService payeSchemesService,
        IValidator<GetAccountPayeSchemesForAuthorisedUserQuery> validator)
    {
        _payeSchemesService = payeSchemesService;
        _validator = validator;
    }

    public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesForAuthorisedUserQuery message, CancellationToken cancellationToken)
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

        var payeSchemes = await _payeSchemesService.GetPayeSchemes(message.AccountId);

        return new GetAccountPayeSchemesResponse
        {
            PayeSchemes = payeSchemes.ToList()
        };
    }
}