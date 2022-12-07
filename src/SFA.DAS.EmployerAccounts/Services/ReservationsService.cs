using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApiRequests.Reservations;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApiResponses.Reservations;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.NLog.Logger;
using ReservationStatus = SFA.DAS.EmployerAccounts.Models.Reservations.ReservationStatus;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ILog _logger;
        public ReservationsService(IOuterApiClient apiClient, ILog logger)
        {
            _logger = logger;
            _outerApiClient = apiClient;
        }

        public async Task<IEnumerable<Reservation>> Get(long accountId)
        {
            IEnumerable<Reservation> reservation = null;

            try
            {
                _logger.Info($"Getting reservations for account ID: {accountId}");

                var reservationsResponse = await _outerApiClient.Get<GetReservationsResponse>(new GetReservationsRequest(accountId));

                if (reservationsResponse != null)
                {
                    reservation =  MapFrom(reservationsResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Could not find reservations for account ID: {accountId} when calling reservations API");
            }

            return reservation;
        }

        private static IEnumerable<Reservation> MapFrom(GetReservationsResponse getReservationsResponse)
        {
            IEnumerable<Reservation> reservation = getReservationsResponse.Reservations.Select(x => new Reservation
            {
                Id = x.Id,
                AccountId = x.AccountId,
                CreatedDate = x.CreatedDate,
                StartDate = x.StartDate,
                ExpiryDate = x.ExpiryDate,
                Course = new Course
                {
                    CourseId = x.Course.CourseId,
                    Title = x.Course.Title,
                    Level = x.Course.Level
                },
                Status = (ReservationStatus)x.Status,
                AccountLegalEntityId = x.AccountLegalEntityId,
                AccountLegalEntityName = x.AccountLegalEntityName
            }).ToList();

            return reservation;
        }
    }
}