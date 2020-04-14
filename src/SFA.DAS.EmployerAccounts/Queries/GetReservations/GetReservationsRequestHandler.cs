using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetReservations
{
    public class GetReservationsRequestHandler : IAsyncRequestHandler<GetReservationsRequest, GetReservationsResponse>
    {
        private readonly IValidator<GetReservationsRequest> _validator;
        private readonly ILog _logger;
        private readonly IReservationsService _service;
        private readonly IHashingService _hashingService;
        private readonly EmployerApprenticeshipsServiceConfiguration _employerApprenticeshipsServiceConfiguration;

        public GetReservationsRequestHandler(
            IValidator<GetReservationsRequest> validator,
            ILog logger,
            IReservationsService service,
            IHashingService hashingService, 
            EmployerApprenticeshipsServiceConfiguration employerApprenticeshipsServiceConfiguration)
        {
            _validator = validator;
            _logger = logger;
            _service = service;
            _hashingService = hashingService;
            _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
        }

        public async Task<GetReservationsResponse> Handle(GetReservationsRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            long accountId = _hashingService.DecodeValue(message.HashedAccountId);

            _logger.Info($"Getting reservations for hashed account id {message.HashedAccountId}");

            try
            {
                var task = _service.Get(accountId);
                if (await Task.WhenAny(task, Task.Delay(_employerApprenticeshipsServiceConfiguration.AddApprenticeCallToActionTimeout)) == task)
                {
                    await task;
                }
                return new GetReservationsResponse
                {
                    Reservations = task.Result
                };
            }
            catch (TimeoutException ex)
            {
                _logger.Error(ex, $"Failed to get Reservations for {message.HashedAccountId}");
                return new GetReservationsResponse
                {
                    HasFailed = true
                };

            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to get Reservations for {message.HashedAccountId}");
                return new GetReservationsResponse
                {
                    HasFailed = true
                };
            }
        }
    }
}
