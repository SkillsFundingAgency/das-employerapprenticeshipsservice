using System.Threading;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Queries.GetVacancies;

public class GetVacanciesRequestHandler : IRequestHandler<GetVacanciesRequest, GetVacanciesResponse>
{
    private readonly IValidator<GetVacanciesRequest> _validator;
    private readonly ILogger<GetVacanciesRequestHandler> _logger;
    private readonly IRecruitService _service;

    public GetVacanciesRequestHandler(
        IValidator<GetVacanciesRequest> validator,
        ILogger<GetVacanciesRequestHandler> logger,
        IRecruitService service)
    {
        _validator = validator;
        _logger = logger;
        _service = service;
    }

    public async Task<GetVacanciesResponse> Handle(GetVacanciesRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.LogInformation("Getting vacancies for hashed account id {HashedAccountId}", message.HashedAccountId);

        try
        {
            return new GetVacanciesResponse
            {
                Vacancies = await _service.GetVacancies(message.HashedAccountId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get vacancies for {HashedAccountId}", message.HashedAccountId);

            return new GetVacanciesResponse
            {
                HasFailed = true
            };
        }
    }
}