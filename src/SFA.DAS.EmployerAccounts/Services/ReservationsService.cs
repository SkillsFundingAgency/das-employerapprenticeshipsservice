﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Reservations;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly IReservationsApiClient _client;

        public ReservationsService(IReservationsApiClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Reservation>> Get(long accountId)
        {
            var reservations = await _client.Get(accountId);
            return JsonConvert.DeserializeObject<IEnumerable<Reservation>>(reservations);
        }
    }
}