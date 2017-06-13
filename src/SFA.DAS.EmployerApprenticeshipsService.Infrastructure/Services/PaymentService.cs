using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.NLog.Logger;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly IEmployerCommitmentApi _commitmentsApiClient;
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;
        private readonly IMapper _mapper;
        private readonly ILog _logger;
        private readonly ICacheProvider _cacheProvider;

        public PaymentService(IPaymentsEventsApiClient paymentsEventsApiClient, IEmployerCommitmentApi commitmentsApiClient, IApprenticeshipInfoServiceWrapper apprenticeshipInfoService, IMapper mapper, ILog logger, ICacheProvider cacheProvider)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _commitmentsApiClient = commitmentsApiClient;
            _apprenticeshipInfoService = apprenticeshipInfoService;
            _mapper = mapper;
            _logger = logger;
            _cacheProvider = cacheProvider;
        }

        public async Task<ICollection<PaymentDetails>> GetAccountPayments(string periodEnd, long employerAccountId)
        {
            var populatedPayments = new List<PaymentDetails>();

            var totalPages = 1;

            for (var index = 1; index <= totalPages; index++)
            {
                var payments = await GetPaymentsPage(employerAccountId, periodEnd, index);

                if (payments == null) continue;

                totalPages = payments.TotalNumberOfPages;

                var paymentDetails = payments.Items.Select(x => _mapper.Map<PaymentDetails>(x)).ToArray();

                foreach (var details in paymentDetails)
                {
                    details.PeriodEnd = periodEnd;

                    await GetProviderDetails(details);
                    await GetApprenticeshipDetails(employerAccountId, details);
                    await GetCourseDetails(details);
                }

                populatedPayments.AddRange(paymentDetails);
            }

            return populatedPayments;
        }

        private async Task GetCourseDetails(PaymentDetails payment)
        {
            payment.CourseName = string.Empty;

            if (payment.StandardCode.HasValue)
            {
                var standard = await GetStandard(payment.StandardCode.Value);

                payment.CourseName = standard?.CourseName;
                payment.CourseLevel = standard?.Level;
            }
            else
            {
                await GetFrameworkCourseDetails(payment);
            }
        }

        private async Task GetFrameworkCourseDetails(PaymentDetails payment)
        {
            if (payment.FrameworkCode.HasValue && payment.ProgrammeType.HasValue && payment.PathwayCode.HasValue)
            {
                var framework = await GetFramework(
                    payment.FrameworkCode.Value,
                    payment.ProgrammeType.Value,
                    payment.PathwayCode.Value);

                payment.CourseName = framework?.FrameworkName;
                payment.CourseLevel = framework?.Level;
                payment.PathwayName = framework?.PathwayName;
            }
        }

        private async Task GetProviderDetails(PaymentDetails payment)
        {
            var provider = await GetProvider(Convert.ToInt32(payment.Ukprn));

            payment.ProviderName = provider?.ProviderName;
        }

        private async Task GetApprenticeshipDetails(long employerAccountId, PaymentDetails payment)
        {
            var apprenticeship = await GetApprenticeship(employerAccountId, payment.ApprenticeshipId);

            if (apprenticeship != null)
            {
                payment.ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}";
                payment.ApprenticeNINumber = apprenticeship.NINumber;
                payment.CourseStartDate = apprenticeship.StartDate;
            }
        }

        private async Task<Apprenticeship> GetApprenticeship(long employerAccountId, long apprenticeshipId)
        {
            try
            {
                return await _commitmentsApiClient.GetEmployerApprenticeship(employerAccountId, apprenticeshipId);
            }
            catch (Exception e)
            {
                _logger.Warn(e, $"Unable to get Apprenticeship with Employer Account ID {employerAccountId} and " +
                                 $"apprenticeship ID {apprenticeshipId} from commitments API.");
            }

            return null;
        }

        private async Task<PageOfResults<Payment>> GetPaymentsPage(long employerAccountId, string periodEnd, int page)
        {
            try
            {
                return await _paymentsEventsApiClient.GetPayments(periodEnd, employerAccountId.ToString(), page);
            }
            catch (WebException ex)
            {
                _logger.Error(ex, $"Unable to get payment information for {periodEnd} accountid {employerAccountId}");
            }

            return null;
        }

        private Task<Domain.Models.ApprenticeshipProvider.Provider> GetProvider(int ukPrn)
        {
            return Task.Run(() =>
            {
                try
                {
                    var providerView = _cacheProvider.Get<ProvidersView>($"{nameof(ProvidersView)}_{ukPrn}");

                    if (providerView == null)
                    {
                        providerView = _apprenticeshipInfoService.GetProvider(ukPrn);
                        if (providerView != null)
                        {
                            _cacheProvider.Set($"{nameof(ProvidersView)}_{ukPrn}", providerView, new TimeSpan(1, 0, 0));
                        }
                    }
                    
                    return providerView?.Provider;
                }
                catch (Exception e)
                {
                    _logger.Warn(e, $"Unable to get provider details with UKPRN {ukPrn} from apprenticeship API.");
                }

                return null;
            });
        }


        private async Task<Standard> GetStandard(long standardCode)
        {
            try
            {
                var standardsView = _cacheProvider.Get<StandardsView>(nameof(StandardsView));

                if (standardsView != null)
                    return standardsView.Standards?.SingleOrDefault(s => s.Code.Equals(standardCode));

                standardsView = await _apprenticeshipInfoService.GetStandardsAsync();

                if (standardsView != null)
                {
                    _cacheProvider.Set(nameof(StandardsView), standardsView, new TimeSpan(1, 0, 0));
                }

                return standardsView?.Standards?.SingleOrDefault(s => s.Code.Equals(standardCode));
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Could not get standards from apprenticeship API.");
            }

            return null;
        }

        private async Task<Framework> GetFramework(int frameworkCode, int programType, int pathwayCode)
        {
            try
            {
                var frameworksView = _cacheProvider.Get<FrameworksView>(nameof(FrameworksView));

                if (frameworksView != null)
                    return frameworksView.Frameworks.SingleOrDefault(f =>
                        f.FrameworkCode.Equals(frameworkCode) &&
                        f.ProgrammeType.Equals(programType) &&
                        f.PathwayCode.Equals(pathwayCode));

                frameworksView = await _apprenticeshipInfoService.GetFrameworksAsync();

                if (frameworksView != null)
                {
                    _cacheProvider.Set(nameof(FrameworksView),frameworksView, new TimeSpan(1,0,0));
                }

                return frameworksView?.Frameworks.SingleOrDefault(f =>
                                     f.FrameworkCode.Equals(frameworkCode) &&
                                     f.ProgrammeType.Equals(programType) &&
                                     f.PathwayCode.Equals(pathwayCode));
            }
            catch (Exception e)
            {
                _logger.Warn(e, "Could not get frameworks from apprenticeship API.");
            }

            return null;
        }
    }
}
