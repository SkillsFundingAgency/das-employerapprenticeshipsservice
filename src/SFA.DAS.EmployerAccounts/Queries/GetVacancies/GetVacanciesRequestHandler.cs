using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetVacancies
{
    public class GetVacanciesRequestHandler : IAsyncRequestHandler<GetVacanciesRequest, GetVacanciesResponse>
    {
        private readonly IValidator<GetVacanciesRequest> _validator;
        private readonly ILog _logger;
        private readonly IRecruitService _service;
        private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;

        public GetVacanciesRequestHandler(
            IValidator<GetVacanciesRequest> validator,
            ILog logger,
            IRecruitService service, EmployerAccountsConfiguration employerAccountsConfiguration)
        {
            _validator = validator;
            _logger = logger;
            _service = service;
            _employerAccountsConfiguration = employerAccountsConfiguration;
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
                var task = await _service.GetVacancies(message.HashedAccountId);
                //if (await Task.WhenAny(task, Task.Delay(_employerAccountsConfiguration.AddApprenticeCallToActionTimeout)) == task)
                //{
                //    await task;
                return new GetVacanciesResponse
                {
                    Vacancies = task
                };
                //}
                //return new GetVacanciesResponse
                //{
                //    HasFailed = true
                //};
            }
            //catch (TimeoutException ex)
            //{
            //    _logger.Error(ex, $"Failed to get vacancies for {message.HashedAccountId}");
            //    return new GetVacanciesResponse
            //    {
            //        HasFailed = true
            //    };
            //}
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
}
