using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Dasync.Collections;
using SFA.DAS.Caches;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;
using AccountTransfer = SFA.DAS.EmployerFinance.Models.Transfers.AccountTransfer;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;

namespace SFA.DAS.EmployerFinance.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly ICommitmentsV2ApiClient _commitmentsV2ApiClient;
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;
        private readonly IMapper _mapper;
        private readonly ILog _logger;
        private readonly IInProcessCache _inProcessCache;
        private readonly IProviderService _providerService;

        public PaymentService(IPaymentsEventsApiClient paymentsEventsApiClient,
            ICommitmentsV2ApiClient commitmentsV2ApiClient, IApprenticeshipInfoServiceWrapper apprenticeshipInfoService,
            IMapper mapper, ILog logger, IInProcessCache inProcessCache, IProviderService providerService)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _commitmentsV2ApiClient = commitmentsV2ApiClient;
            _apprenticeshipInfoService = apprenticeshipInfoService;
            _mapper = mapper;
            _logger = logger;
            _inProcessCache = inProcessCache;
            _providerService = providerService;
        }

        public async Task<ICollection<PaymentDetails>> GetAccountPayments(string periodEnd, long employerAccountId, Guid correlationId)
        {
            var populatedPayments = new List<PaymentDetails>();

            var totalPages = 1;

            for (var index = 1; index <= totalPages; index++)
            {
                var payments = await GetPaymentsPage(employerAccountId, periodEnd, index);

                if (payments == null) continue;

                totalPages = payments.TotalNumberOfPages;

                var paymentDetails = payments.Items.Select(x => _mapper.Map<PaymentDetails>(x)).ToArray();

                _logger.Info($"Fetching provider and apprenticeship for AccountId = {employerAccountId}, periodEnd={periodEnd}, correlationId = {correlationId}");

                var ukprnList = paymentDetails.Select(pd => pd.Ukprn).Distinct();
                var apprenticeshipIdList = paymentDetails.Select(pd => pd.ApprenticeshipId).Distinct();

                var getProviderDetailsTask = GetProviderDetailsDict(ukprnList);
                var getApprenticeDetailsTask = GetApprenticeshipDetailsDict(employerAccountId, apprenticeshipIdList);

                await Task.WhenAll(getProviderDetailsTask, getApprenticeDetailsTask);

                var apprenticeshipDetails = getApprenticeDetailsTask.Result;
                var providerDetails = getProviderDetailsTask.Result;

                _logger.Info($"Fetched provider and apprenticeship for AccountId = {employerAccountId}, periodEnd={periodEnd}, correlationId = {correlationId} - with {providerDetails.Count} providers and {apprenticeshipDetails.Count} apprenticeship details");

                foreach (var details in paymentDetails)
                {
                    details.PeriodEnd = periodEnd;
                    var getCourseDetailsTask = GetCourseDetails(details);

                    providerDetails.TryGetValue(details.Ukprn, out var provider);
                    details.ProviderName = provider?.Name;
                    details.IsHistoricProviderName = provider?.IsHistoricProviderName ?? false;

                    if (apprenticeshipDetails.TryGetValue(details.ApprenticeshipId, out var apprenticeship))
                    {
                        details.ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}";
                        details.CourseStartDate = apprenticeship.StartDate;
                    }

                    await getCourseDetailsTask;
                }

                populatedPayments.AddRange(paymentDetails);

                _logger.Info($"Populated payements page {index} of {totalPages} for AccountId = {employerAccountId}, periodEnd={periodEnd}, correlationId = {correlationId}");
            }

            return populatedPayments;
        }

        private async Task<ConcurrentDictionary<long, Models.ApprenticeshipProvider.Provider>> GetProviderDetailsDict(IEnumerable<long> ukprnList)
        {
            var maxConcurrentThreads = 50;
            var resultProviders = new ConcurrentDictionary<long, Models.ApprenticeshipProvider.Provider>();

            await ukprnList
                .ParallelForEachAsync(async ukprn =>
                {
                    if (!resultProviders.ContainsKey(ukprn))
                    {
                        var provider = await _providerService.Get(ukprn);
                        resultProviders.TryAdd(ukprn, provider);
                    }
                }, maxDegreeOfParallelism: maxConcurrentThreads);

            return resultProviders;
        }

        private async Task<ConcurrentDictionary<long, GetApprenticeshipResponse>> GetApprenticeshipDetailsDict(long employerAccountId, IEnumerable<long> apprenticeshipIdList)
        {
            var resultApprenticeships = new ConcurrentDictionary<long, GetApprenticeshipResponse>();

            var maxConcurrentThreads = 50;
            await apprenticeshipIdList
                .ParallelForEachAsync(async apprenticeshipId =>
                {
                    var apprenticeship = await GetApprenticeship(employerAccountId, apprenticeshipId);
                    if (apprenticeship != null)
                    {
                        resultApprenticeships.TryAdd(apprenticeship.Id, apprenticeship);
                    }
                }, maxDegreeOfParallelism: maxConcurrentThreads);

            return resultApprenticeships;
        }

        public async Task<IEnumerable<AccountTransfer>> GetAccountTransfers(string periodEnd, long receiverAccountId, Guid correlationId)
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
            else if (payment.FrameworkCode.HasValue && payment.FrameworkCode > 0)
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

        private async Task<GetApprenticeshipResponse> GetApprenticeship(long employerAccountId, long apprenticeshipId)
        {
            try
            {
                return await _commitmentsV2ApiClient.GetApprenticeship(apprenticeshipId);
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