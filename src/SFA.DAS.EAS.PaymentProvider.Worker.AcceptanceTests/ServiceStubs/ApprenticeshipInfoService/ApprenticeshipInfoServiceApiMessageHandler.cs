﻿using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Net;


namespace SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests.ServiceStubs.ApprenticeshipInfoService
{
    public class ApprenticeshipInfoServiceApiMessageHandler : ApiMessageHandlers
    {
        private readonly ILog _logger;

        public string StandardCode { get; set; }

        public ApprenticeshipInfoServiceApiMessageHandler(string baseAddress, ILog logger) : base(baseAddress)
        {
            _logger = logger;
        }

        public void SetStandards(HttpStatusCode statusCode, IEnumerable<StandardSummary> response)
        {
            SetupGet($"/standards", statusCode, response);
        }
    }
}
