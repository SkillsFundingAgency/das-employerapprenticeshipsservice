using System.Threading;
using AutoMapper;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Finance;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Finance;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using DasEnglishFraction = SFA.DAS.EmployerAccounts.Models.Levy.DasEnglishFraction;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;

public class GetEmployerEnglishFractionHistoryQueryHandler : IRequestHandler<GetEmployerEnglishFractionHistoryQuery, GetEmployerEnglishFractionHistoryResponse>
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

    public async Task<GetEmployerEnglishFractionHistoryResponse> Handle(GetEmployerEnglishFractionHistoryQuery message, CancellationToken cancellationToken)
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
        var schemeInformation = await _mediator.Send(new GetPayeSchemeInUseQuery {Empref = message.EmpRef}, cancellationToken);

        return new GetEmployerEnglishFractionHistoryResponse 
        {
            Fractions = _mapper.Map<List<DasEnglishFraction>>(englishFractionHistory.Fractions), 
            EmpRef = message.EmpRef,
            EmpRefAddedDate = schemeInformation.PayeScheme.AddedDate
        };
    }
}