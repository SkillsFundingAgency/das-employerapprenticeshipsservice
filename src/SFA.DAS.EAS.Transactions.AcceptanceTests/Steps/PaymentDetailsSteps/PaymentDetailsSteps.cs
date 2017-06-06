using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.TestCommon.Extensions;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.PaymentDetailsSteps
{
    [Binding]
    public class PaymentDetailsSteps : TechTalk.SpecFlow.Steps
    {
        private static PaymentTestData _testData;
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private static Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private static Mock<IEventsApi> _eventsApi;
        private static Mock<IPaymentsEventsApiClient> _paymentEventsApi;
        private static Mock<IEmployerCommitmentApi> _employerCommitmentApi;
        private static Mock<IApprenticeshipInfoServiceWrapper> _apprenticeshipInfoService;
        private static Mock<ICacheProvider> _cacheProvider;

        [BeforeScenario]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _eventsApi = new Mock<IEventsApi>();
            _paymentEventsApi = new Mock<IPaymentsEventsApiClient>();
            _employerCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _apprenticeshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();
            _cacheProvider = new Mock<ICacheProvider>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi);
            _container.Inject(typeof(IPaymentsEventsApiClient), _paymentEventsApi.Object);
            _container.Inject(typeof(IEmployerCommitmentApi), _employerCommitmentApi.Object);
            _container.Inject(typeof(IApprenticeshipInfoServiceWrapper), _apprenticeshipInfoService.Object);
            _container.Inject(typeof(ICacheProvider), _cacheProvider.Object);

            _cacheProvider.Setup(x => x.Get<Standard>(It.IsAny<string>())).Returns((Standard) null);
            _cacheProvider.Setup(x => x.Get<Framework>(It.IsAny<string>())).Returns((Framework) null);

            RegisterMapper();

            ScenarioContext.Current.Clear();

            _testData = new PaymentTestData(_container);
            _testData.PopulateTestData();
        }

        [AfterScenario]
        public static void TearDown()
        {
            _container.Dispose();
        }
        
        [Given(@"I have an apprenticeship")]
        public void GivenIHaveAnApprenticeship()
        {
            _employerCommitmentApi.Setup(x => x.GetEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_testData.Apprenticeship);
        }

        [Given(@"I have a standard")]
        public void GivenIHaveAStandard()
        {
            _apprenticeshipInfoService.Setup(x => x.GetStandardsAsync(false))
                .ReturnsAsync(_testData.StandardsView);
        }

        [Given(@"I have a framework")]
        public void GivenIHaveAFramework()
        {
            _apprenticeshipInfoService.Setup(x => x.GetFrameworksAsync(false))
                .ReturnsAsync(_testData.FrameworksView);
        }

        [Given(@"I have a provider")]
        public void GivenIHaveAProvider()
        {
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<long>()))
                .Returns(_testData.ProvidersView);
        }

        [When(@"I make a payment for the apprenticeship standard")]
        public void WhenIMakeAPaymentForTheApprenticeshipStandard()
        {
            _paymentEventsApi.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), 1))
                .ReturnsAsync(new PageOfResults<Provider.Events.Api.Types.Payment> { Items = new[] { _testData.StandardPayment } });
        }

        [When(@"I make a payment for the apprenticeship framework")]
        public void WhenIMakeAPaymentForTheApprenticeshipFramework()
        {
            _paymentEventsApi.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), 1))
                .ReturnsAsync(new PageOfResults<Provider.Events.Api.Types.Payment> { Items = new[] { _testData.FrameworkPayment } });
        }
        
        [When(@"I make a co-investment payment for the apprenticeship")]
        public void WhenIMakeACo_InvestmentPaymentForTheApprenticeship()
        {
            _paymentEventsApi.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), 1))
                .ReturnsAsync(new PageOfResults<Provider.Events.Api.Types.Payment> { Items = new[]
                {
                    _testData.StandardPayment,
                    _testData.StandardSFACoInvestmentPayment,
                    _testData.StandardEmployerCoInvestmentPayment
                } });
        }

        [When(@"payment details are updated")]
        public void WhenPaymentDetailsAreUpdated()
        {
            var mediator = _container.GetInstance<IMediator>();

            mediator.SendAsync(new RefreshPaymentDataCommand
            {
                AccountId = _testData.AccountId,
                PeriodEnd = _testData.PeriodEnd.Id,
                PaymentUrl = "test"
            }).Wait();
        }

        [Then(@"the updated payment details should be stored")]
        public void ThenTheUpdatedPaymentDetailsShouldBeStored()
        {
            var transactionMonthStart = _testData.PaymentCollectionDate.StartOfMonth();
            var transactionMonthEnd = _testData.PaymentCollectionDate.EndOfMonth();

            var repository = _container.GetInstance<ITransactionRepository>();

            var transactions = repository.GetAccountTransactionByProviderAndDateRange(_testData.AccountId, _testData.StandardPayment.Ukprn, transactionMonthStart, transactionMonthEnd).Result;

            var paymentTransaction = transactions.OfType<PaymentTransactionLine>().First();

            Assert.AreEqual(_testData.AccountId, paymentTransaction.AccountId);
            Assert.AreEqual(_testData.StandardPayment.Amount, paymentTransaction.LineAmount * -1);
            Assert.AreEqual(TransactionItemType.Payment, paymentTransaction.TransactionType);
            Assert.AreEqual(_testData.StandardPayment.Ukprn, paymentTransaction.UkPrn);
            Assert.AreEqual(_testData.StandardPayment.CollectionPeriod.Id, paymentTransaction.PeriodEnd);
        }

        [Then(@"the apprenticeship course details are stored")]
        public void ThenTheApprenticeshipCourseDetailsAreStored()
        {
            var transactionMonthStart = _testData.PaymentCollectionDate.StartOfMonth();
            var transactionMonthEnd = _testData.PaymentCollectionDate.EndOfMonth();

            var repository = _container.GetInstance<ITransactionRepository>();

            var transactions = repository.GetAccountTransactionByProviderAndDateRange(_testData.AccountId, _testData.StandardPayment.Ukprn, transactionMonthStart, transactionMonthEnd)
                                         .Result;

            var paymentTransaction = transactions.OfType<PaymentTransactionLine>().First();

            var expectedStartDate = _testData.Apprenticeship.StartDate?.ToShortDateString() ?? "expectedDateNotFound";
            var courseStartDate = paymentTransaction.CourseStartDate?.ToShortDateString() ?? "courseStartDateNotFound";

            Assert.AreEqual(_testData.Standard.Level, paymentTransaction.CourseLevel);
            Assert.AreEqual(_testData.Standard.CourseName, paymentTransaction.CourseName);
            Assert.AreEqual(expectedStartDate, courseStartDate);
        }

        [Then(@"the apprenticeship course details are stored with coinvestment figures")]
        public void ThenTheApprenticeshipCourseDetailsAreStoredWithCoinvestmentFigures()
        {
            var transactionMonthStart = _testData.PaymentCollectionDate.StartOfMonth();
            var transactionMonthEnd = _testData.PaymentCollectionDate.EndOfMonth();

            var repository = _container.GetInstance<ITransactionRepository>();
            var transactions = repository.GetAccountTransactionByProviderAndDateRange(_testData.AccountId, _testData.StandardPayment.Ukprn, transactionMonthStart, transactionMonthEnd).Result;
           
            var paymentTransaction = transactions.OfType<PaymentTransactionLine>().First();

            var expectedStartDate = _testData.Apprenticeship.StartDate?.ToShortDateString() ?? "expectedDateNotFound";
            var courseStartDate = paymentTransaction.CourseStartDate?.ToShortDateString() ?? "courseStartDateNotFound";

            Assert.AreEqual(_testData.Standard.Level, paymentTransaction.CourseLevel);
            Assert.AreEqual(_testData.Standard.CourseName, paymentTransaction.CourseName);
            Assert.AreEqual(expectedStartDate, courseStartDate);
            Assert.AreEqual(_testData.StandardSFACoInvestmentPayment.Amount * -1, paymentTransaction.SfaCoInvestmentAmount);
            Assert.AreEqual(_testData.StandardEmployerCoInvestmentPayment.Amount * -1, paymentTransaction.EmployerCoInvestmentAmount);
        }

        [Then(@"the apprenticeship learner details are stored")]
        public void ThenTheApprenticeshipLearnerDetailsAreStored()
        {
            var transactionMonthStart = _testData.PaymentCollectionDate.StartOfMonth();
            var transactionMonthEnd = _testData.PaymentCollectionDate.EndOfMonth();

            var repository = _container.GetInstance<ITransactionRepository>();

            var transactions = repository.GetAccountTransactionByProviderAndDateRange(_testData.AccountId, _testData.StandardPayment.Ukprn, transactionMonthStart, transactionMonthEnd).Result;

            var paymentTransaction = transactions.OfType<PaymentTransactionLine>().First();

            Assert.AreEqual($"{_testData.Apprenticeship.FirstName} {_testData.Apprenticeship.LastName}", paymentTransaction.ApprenticeName);
            Assert.AreEqual(_testData.Apprenticeship.NINumber, paymentTransaction.ApprenticeNINumber);
        }

        [Then(@"the apprenticeship pathway details are stored")]
        public void ThenTheApprenticeshipPathwayDetailsAreStored()
        {
            var transactionMonthStart = _testData.PaymentCollectionDate.StartOfMonth();
            var transactionMonthEnd = _testData.PaymentCollectionDate.EndOfMonth();

            var repository = _container.GetInstance<ITransactionRepository>();

            var transactions = repository.GetAccountTransactionByProviderAndDateRange(_testData.AccountId, _testData.FrameworkPayment.Ukprn, transactionMonthStart, transactionMonthEnd).Result;

            var paymentTransaction = transactions.OfType<PaymentTransactionLine>().First();

            Assert.AreEqual(_testData.Framework.PathwayName, paymentTransaction.PathwayName);
        }

        private static void RegisterMapper()
        {
            var profiles = Assembly.Load("SFA.DAS.EAS.Infrastructure").GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t)).ToList();

            var config = new MapperConfiguration(cfg =>
            {
                profiles.ForEach(cfg.AddProfile);
            });

            var mapper = config.CreateMapper();
            _container.Inject(typeof(IMapper), mapper);
        }
    }
}
