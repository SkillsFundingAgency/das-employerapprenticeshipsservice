using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetVacancies;

public class GetVacanciesRequestHandler : IAsyncRequestHandler<GetVacanciesRequest, GetVacanciesResponse>
{
    private readonly IValidator<GetVacanciesRequest> _validator;
    private readonly ILog _logger;
    private readonly IRecruitService _service;

    public GetVacanciesRequestHandler(
        IValidator<GetVacanciesRequest> validator,
        ILog logger,
        IRecruitService service)
    {
        _validator = validator;
        _logger = logger;
        _service = service;
    }

    public async Task<GetVacanciesResponse> Handle(GetVacanciesRequest message)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.Info($"Getting vacancies for hashed account id {message.HashedAccountId}");

        try
        {
            return new GetVacanciesResponse
            {
                Vacancies = await _service.GetVacancies(message.HashedAccountId)
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to get vacancies for {message.HashedAccountId}");
            return new GetVacanciesResponse
            {
                HasFailed = true
            };
        }
    }
}