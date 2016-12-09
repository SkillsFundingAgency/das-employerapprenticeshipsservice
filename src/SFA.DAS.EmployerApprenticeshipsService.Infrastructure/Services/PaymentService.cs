using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using NLog;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.Payments.Events.Api.Client;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly ICommitmentsApi _commitmentsApiClient;
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PaymentService(
            IPaymentsEventsApiClient paymentsEventsApiClient,
            ICommitmentsApi commitmentsApiClient,
            IApprenticeshipInfoServiceWrapper apprenticeshipInfoService,
            IMapper mapper,
            ILogger logger)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _commitmentsApiClient = commitmentsApiClient;
            _apprenticeshipInfoService = apprenticeshipInfoService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ICollection<PaymentDetails>> GetAccountPayments(string periodEnd, string employerAccountId)
        {
            var payments = await GetPayments(periodEnd, employerAccountId);

            if (!payments.Any())
            {
                return new List<PaymentDetails>();
            }

            foreach (var payment in payments)
            {
                payment.PeriodEnd = periodEnd;

                var provider = await GetProvider(Convert.ToInt32(payment.Ukprn));

                if (provider != null)
                {
                    payment.ProviderName = provider.ProviderName;

                    var apprenticeship = await GetApprenticeship(provider.Id, payment.ApprenticeshipId);

                    if (apprenticeship != null)
                    {
                        payment.ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}";
                        payment.ApprenticeNINumber = apprenticeship.NINumber;
                    }
                }

                if (payment.StandardCode.HasValue)
                {
                    payment.CourseName = await GetStandardCourseName(payment.StandardCode.Value);
                }
                else if (payment.FrameworkCode.HasValue && payment.ProgrammeType.HasValue && payment.PathwayCode.HasValue)
                {
                    payment.CourseName = await GetFrameworkCourseName(
                        payment.FrameworkCode.Value,
                        payment.ProgrammeType.Value,
                        payment.PathwayCode.Value);
                }
                else
                {
                    payment.CourseName = string.Empty;
                }
            }

            return payments;
        }

        private async Task<Apprenticeship> GetApprenticeship(int providerId, long apprenticeshipId)
        {
            try
            {
                return await _commitmentsApiClient.GetProviderApprenticeship(providerId, apprenticeshipId);
            }
            catch (WebException e)
            {
                _logger.Error(e, $"Unable to get Apprenticeship with provider ID {providerId} and " +
                                 $"apprenticeship ID {apprenticeshipId} from commitments API.");
            }

            return null;
        }

        private async Task<ICollection<PaymentDetails>> GetPayments(string periodEnd, string employerAccountId)
        {
            var paymentDetails = new List<PaymentDetails>();

            try
            {
                var payments = await _paymentsEventsApiClient.GetPayments(periodEnd, employerAccountId);

                if (payments == null)
                {
                    return paymentDetails;
                }

                paymentDetails = payments.Items.Select(x => _mapper.Map<PaymentDetails>(x)).ToList();
            }
            catch (WebException ex)
            {
                _logger.Error(ex, $"Unable to get payment information for {periodEnd} accountid {employerAccountId}");
            }

            return paymentDetails;
        }

        private Task<Provider> GetProvider(int ukPrn)
        {
            return Task.Run(() =>
            {
                try
                {
                    var providerView = _apprenticeshipInfoService.GetProvider(ukPrn);
                    return providerView?.Providers?.FirstOrDefault();
                }
                catch (WebException e)
                {
                    _logger.Error(e, $"Unable to get provider details with UKPRN {ukPrn} from apprenticeship API.");
                }

                return null;
            });
        }

        private async Task<string> GetStandardCourseName(long standardCode)
        {
            try
            {
                var standardsView = await _apprenticeshipInfoService.GetStandardsAsync();

                return standardsView.Standards.SingleOrDefault(s =>
                                             s.Code.Equals(standardCode))?.Title ?? string.Empty;
            }
            catch (WebException e)
            {
                _logger.Error(e, "Could not get standards from apprenticeship API.");
            }

            return null;
        }

        private async Task<string> GetFrameworkCourseName(int frameworkCode, int programType, int pathwayCode)
        {
            try
            {
                var frameworksView = await _apprenticeshipInfoService.GetFrameworksAsync();

                return frameworksView.Frameworks.SingleOrDefault(f =>
                                     f.FrameworkCode.Equals(frameworkCode) &&
                                     f.ProgrammeType.Equals(programType) &&
                                     f.PathwayCode.Equals(pathwayCode))?.Title ?? string.Empty;
            }
            catch (WebException e)
            {
                _logger.Error(e, "Could not get frameworks from apprenticeship API.");
            }

            return null;
        }
    }
}
