using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Caches;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;
using AccountTransfer = SFA.DAS.EmployerFinance.Models.Transfers.AccountTransfer;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ApprenticeshipCache
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NINumber { get; set; }
        public DateTime? StartDate { get; set; }
        public long ApprenticeshipId { get; set; }
    }
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly IEmployerCommitmentApi _commitmentsApiClient;
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;
        private readonly IMapper _mapper;
        private readonly ILog _logger;
        private readonly IInProcessCache _inProcessCache;
        private readonly IProviderService _providerService;
        private readonly List<ApprenticeshipCache> _apprenticeships;

        public PaymentService(IPaymentsEventsApiClient paymentsEventsApiClient,
            IEmployerCommitmentApi commitmentsApiClient, IApprenticeshipInfoServiceWrapper apprenticeshipInfoService,
            IMapper mapper, ILog logger, IInProcessCache inProcessCache, IProviderService providerService)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _commitmentsApiClient = commitmentsApiClient;
            _apprenticeshipInfoService = apprenticeshipInfoService;
            _mapper = mapper;
            _logger = logger;
            _inProcessCache = inProcessCache;            
            _providerService = providerService;
            _apprenticeships = new List<ApprenticeshipCache>();
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

        public async Task<IEnumerable<AccountTransfer>> GetAccountTransfers(string periodEnd, long receiverAccountId)
        {
            var pageOfTransfers =
                await _paymentsEventsApiClient.GetTransfers(periodEnd, receiverAccountId: receiverAccountId);

            var transfers = new List<AccountTransfer>();

            foreach (var item in pageOfTransfers.Items)
            {
                transfers.Add(new AccountTransfer
                {
                    SenderAccountId = item.SenderAccountId,
                    ReceiverAccountId = item.ReceiverAccountId,
                    PeriodEnd = periodEnd,
                    Amount = item.Amount,
                    ApprenticeshipId = item.CommitmentId,
                    Type = item.Type.ToString(),
                    RequiredPaymentId = item.RequiredPaymentId
                });
            }

            return transfers;
        }

        private async Task GetCourseDetails(PaymentDetails payment)
        {
            payment.CourseName = string.Empty;

            if (payment.StandardCode.HasValue && payment.StandardCode > 0)
            {
                var standard = await GetStandard(payment.StandardCode.Value);

                payment.CourseName = standard?.CourseName;
                payment.CourseLevel = standard?.Level;
            }
            else if(payment.FrameworkCode.HasValue && payment.FrameworkCode > 0)
            {
                await GetFrameworkCourseDetails(payment);
            }
            else
            {
                _logger.Warn($"No framework code or standard code set on payment. Cannot get course details. PaymentId: {payment.Id}");
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
            var provider = await _providerService.Get(payment.Ukprn);
            payment.ProviderName = provider?.Name;
            payment.IsHistoricProviderName = provider?.IsHistoricProviderName ?? false;
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

        private async Task<ApprenticeshipCache> GetApprenticeship(long employerAccountId, long apprenticeshipId)
        {
            try
            {
                var apprenticeshipFromCache =_apprenticeships.FirstOrDefault(x => x.ApprenticeshipId == apprenticeshipId);

                if (apprenticeshipFromCache == null)
                {
                    var apprenticeship = await _commitmentsApiClient.GetEmployerApprenticeship(employerAccountId, apprenticeshipId);
                    _apprenticeships.Add(new ApprenticeshipCache
                    {
                        FirstName = apprenticeship.FirstName,
                        LastName = apprenticeship.LastName,
                        NINumber = apprenticeship.NINumber,
                        StartDate = apprenticeship.StartDate
                    });
                }
                else
                {
                    _logger.Info("Found apprenticeship from cache :" + apprenticeshipFromCache.ApprenticeshipId);
                }

                _logger.Info("Cache apprenticeship size :" + _apprenticeships.Count());
                return apprenticeshipFromCache;
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
                return await _paymentsEventsApiClient.GetPayments(periodEnd, employerAccountId.ToString(), page, null);
            }
            catch (WebException ex)
            {
                _logger.Error(ex, $"Unable to get payment information for {periodEnd} accountid {employerAccountId}");
            }

            return null;
        }        

        private async Task<Standard> GetStandard(long standardCode)
        {
            try
            {
                var standardsView = _inProcessCache.Get<StandardsView>(nameof(StandardsView));

                if (standardsView != null)
                    return standardsView.Standards?.SingleOrDefault(s => s.Code.Equals(standardCode));

                standardsView = await _apprenticeshipInfoService.GetStandardsAsync();

                if (standardsView != null)
                {
                    _inProcessCache.Set(nameof(StandardsView), standardsView, new TimeSpan(1, 0, 0));
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
                var frameworksView = _inProcessCache.Get<FrameworksView>(nameof(FrameworksView));

                if (frameworksView != null)
                    return frameworksView.Frameworks.SingleOrDefault(f =>
                        f.FrameworkCode.Equals(frameworkCode) &&
                        f.ProgrammeType.Equals(programType) &&
                        f.PathwayCode.Equals(pathwayCode));

                frameworksView = await _apprenticeshipInfoService.GetFrameworksAsync();

                if (frameworksView != null)
                {
                    _inProcessCache.Set(nameof(FrameworksView), frameworksView, new TimeSpan(1, 0, 0));
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