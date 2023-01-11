using AutoMapper;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiRequests.Finance;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiResponses.Finance;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.Validation;
using DasEnglishFraction = SFA.DAS.EmployerAccounts.Models.Levy.DasEnglishFraction;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;

public class GetEmployerEnglishFractionHistoryQueryHandler : IAsyncRequestHandler<GetEmployerEnglishFractionHistoryQuery, GetEmployerEnglishFractionHistoryResponse>
{
    private readonly IValidator<GetEmployerEnglishFractionHistoryQuery> _validator;
    private readonly IOuterApiClient _outerApiClient;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public GetEmployerEnglishFractionHistoryQueryHandler(IValidator<GetEmployerEnglishFractionHistoryQuery> validator, IOuterApiClient outerApiClient, IMediator mediator, IMapper mapper)
    {
        _validator = validator;
        _outerApiClient = outerApiClient;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<GetEmployerEnglishFractionHistoryResponse> Handle(GetEmployerEnglishFractionHistoryQuery message)
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

        var englishFractionHistory = await _outerApiClient.Get<GetEnglishFractionHistoryResponse>(new GetEnglishFractionHistoryRequest(message.HashedAccountId, message.EmpRef));
        var schemeInformation = await _mediator.SendAsync(new GetPayeSchemeInUseQuery {Empref = message.EmpRef});

        return new GetEmployerEnglishFractionHistoryResponse 
        {
            Fractions = _mapper.Map<List<DasEnglishFraction>>(englishFractionHistory.Fractions), 
            EmpRef = message.EmpRef,
            EmpRefAddedDate = schemeInformation.PayeScheme.AddedDate
        };
    }
}