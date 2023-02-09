using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetVacancies;

public class GetVacanciesRequestHandler : IRequestHandler<GetVacanciesRequest, GetVacanciesResponse>
{
    private readonly IValidator<GetVacanciesRequest> _validator;
    private readonly ILogger<GetVacanciesRequestHandler> _logger;
    private readonly IRecruitService _service;
    private readonly IEncodingService _encodingService;

    public GetVacanciesRequestHandler(
        IValidator<GetVacanciesRequest> validator,
        ILogger<GetVacanciesRequestHandler> logger,
        IRecruitService service,
        IEncodingService encodingService)
    {
        _validator = validator;
        _logger = logger;
        _service = service;
        _encodingService = encodingService;
    }

    public async Task<GetVacanciesResponse> Handle(GetVacanciesRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);
        var hashedAccountId = _encodingService.Encode(message.AccountId, EncodingType.AccountId);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.LogInformation($"Getting vacancies for hashed account id {message.AccountId}");

        try
        {
            return new GetVacanciesResponse
            {
                Vacancies = await _service.GetVacancies(hashedAccountId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get vacancies for {message.AccountId}");
            return new GetVacanciesResponse
            {
                HasFailed = true
            };
        }
    }
}