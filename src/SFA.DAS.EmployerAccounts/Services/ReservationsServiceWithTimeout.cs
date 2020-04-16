using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ReservationsServiceWithTimeout : IReservationsService
    {
        private readonly IReservationsService _service;
        private readonly ILog _logger;
        private TimeoutPolicy TimeoutPolicy { get; }

        public ReservationsServiceWithTimeout(IReservationsService service, ILog logger)
        {
            _service = service;
            _logger = logger;
            TimeoutPolicy= PollyTimeoutExtensions.GetTimeoutPolicy(_logger);
        }

        public async Task<IEnumerable<Reservation>> Get(long accountId)
        {
            //var response = await _pollyPolicy.ExecuteAsync(() => _httpClient.PostAsync(url, null));
            //response.EnsureSuccessStatusCode();
            //return await response.Content.ReadAsStringAsync();
            var response = await TimeoutPolicy.ExecuteAsync(() =>
                _service.Get(accountId)); 
            //response.EnsureSuccessStatusCode();
            return response;//.Content.ReadAsStringAsync();
            //return await TimeoutPolicy.ExecuteAsync(async token =>
            //         await _service.Get(accountId), CancellationToken.None);
            //return await TimeoutPolicy.ExecuteAsync(context =>
            //    _service.Get(accountId), CancellationToken.None, true);

            //var result1 = await TimeoutPolicy.ExecuteAndCaptureAsync(async (ct) =>
            //{
            //    await _service.Get(accountId);
            //}, CancellationToken.None);

            //return _service.Get(accountId);
        }
    }
}