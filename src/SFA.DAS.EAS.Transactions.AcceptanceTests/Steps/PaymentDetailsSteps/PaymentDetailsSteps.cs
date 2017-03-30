using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.TestCommon.DependencyResolution;
using SFA.DAS.EAS.Web;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Messaging;
using SFA.DAS.Payments.Events.Api.Client;
using SFA.DAS.Payments.Events.Api.Types;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.Transactions.AcceptanceTests.Steps.PaymentDetailsSteps
{
    [Binding]
    public class PaymentDetailsSteps : TechTalk.SpecFlow.Steps
    {
        private static IContainer _container;
        private static Mock<IMessagePublisher> _messagePublisher;
        private static Mock<IOwinWrapper> _owinWrapper;
        private static Mock<ICookieService> _cookieService;
        private static Mock<IEventsApi> _eventsApi;
        private static Mock<IPaymentsEventsApiClient> _paymentEventsApi;
        private static Mock<IEmployerCommitmentApi> _employerCommitmentApi;
        private static Mock<IApprenticeshipInfoServiceWrapper> _apprenticeshipInfoService;

        [BeforeFeature]
        public static void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _cookieService = new Mock<ICookieService>();
            _eventsApi = new Mock<IEventsApi>();
            _paymentEventsApi = new Mock<IPaymentsEventsApiClient>();
            _employerCommitmentApi = new Mock<IEmployerCommitmentApi>();
            _apprenticeshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();
            
            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper, _cookieService, _eventsApi);
            _container.Inject(typeof(IPaymentsEventsApiClient), _paymentEventsApi.Object);
            _container.Inject(typeof(IEmployerCommitmentApi), _employerCommitmentApi.Object);
            _container.Inject(typeof(IApprenticeshipInfoServiceWrapper), _apprenticeshipInfoService.Object);

            RegisterMapper();
        }

        [AfterFeature]
        public static void TearDown()
        {
            _container.Dispose();
        }

        [Given(@"I have an apprenticeship")]
        public void GivenIHaveAnApprenticeship()
        {
            var accountId = (long) ScenarioContext.Current["AccountId"];

            var apprenticeship = new Apprenticeship
            {
                Id = 1,
                EmployerAccountId = accountId,
                FirstName = "Bob",
                LastName = "Green",
                NINumber = "AB 12 34 56 C",
                StartDate = DateTime.Now.AddDays(20)
            };
            ScenarioContext.Current["apprenticeship"] = apprenticeship;

            _employerCommitmentApi.Setup(x => x.GetEmployerApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(apprenticeship);
        }

        [Given(@"I have a standard")]
        public void GivenIHaveAStandard()
        {
            var standard = new Standard
            {
                Code = 20,
                Level = 3,
                Title = "Testing"
            };
            ScenarioContext.Current["standard"] = standard;

            _apprenticeshipInfoService.Setup(x => x.GetStandardsAsync(false))
                .ReturnsAsync(new StandardsView
                {
                    CreationDate = DateTime.Now,
                    Standards = new List<Standard> { standard }
                });
        }

        [Given(@"I have a provider")]
        public void GivenIHaveAProvider()
        {
            var provider = new Provider { ProviderName = "Test Corp" };

            ScenarioContext.Current["provider"] = provider;

            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<long>()))
                                      .Returns(new ProvidersView
                                      {
                                          CreatedDate = DateTime.Now,
                                          Provider = provider
                                      });
        }

        [When(@"I make a payment for the apprenticeship")]
        public void WhenIMakeAPaymentForTheApprenticeship()
        {
            var standard = (Standard) ScenarioContext.Current["standard"];
            var apprenticeship = (Apprenticeship)ScenarioContext.Current["apprenticeship"];
            var accountId = (long)ScenarioContext.Current["AccountId"];

            var dasLevyRepository = _container.GetInstance<IDasLevyRepository>();

            var periodEnd = dasLevyRepository.GetLatestPeriodEnd().Result;

            if (periodEnd == null)
            {
                periodEnd = new PeriodEnd
                {
                    CalendarPeriod = new CalendarPeriod {Month = 1, Year = 2016},
                    CompletionDateTime = DateTime.Now,
                    Id = "1617-R12",
                    ReferenceData =
                        new ReferenceDataDetails
                        {
                            AccountDataValidAt = DateTime.Now,
                            CommitmentDataValidAt = DateTime.Now
                        },
                    Links = new PeriodEndLinks {PaymentsForPeriod = ""}
                };

                dasLevyRepository.CreateNewPeriodEnd(periodEnd).Wait();
            }

            ScenarioContext.Current["periodEnd"] = periodEnd;

            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Ukprn = 100,
                StandardCode = standard.Code,
                ApprenticeshipId = apprenticeship.Id,
                DeliveryPeriod = new CalendarPeriod { Month = 3, Year = 2017 },
                Amount = 200,
                CollectionPeriod = new NamedCalendarPeriod { Id = periodEnd.Id, Month = 3, Year = 2017 },
                TransactionType = TransactionType.Learning,
                EvidenceSubmittedOn = DateTime.Now.AddDays(-5),
                Uln = 123,
                EmployerAccountVersion = "1.0",
                EmployerAccountId = accountId.ToString(),
                FundingSource = FundingSource.Levy,
                ApprenticeshipVersion = "1.0"
            };

            ScenarioContext.Current["payment"] = payment;

            _paymentEventsApi.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), 1))
                .ReturnsAsync(new PageOfResults<Payment> { Items = new[] { payment } });
        }


        [When(@"payment details are updated")]
        public void WhenPaymentDetailsAreUpdated()
        {
            var accountId = (long) ScenarioContext.Current["AccountId"];
            var mediator = _container.GetInstance<IMediator>();

            mediator.SendAsync(new RefreshPaymentDataCommand
            {
                AccountId = accountId,
                PeriodEnd = "1617-R12",
                PaymentUrl = "test"
            }).Wait();
        }
        
        [Then(@"the updated payment details should be stored")]
        public void ThenTheUpdatedPaymentDetailsShouldBeStored()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var payment = (Payment)ScenarioContext.Current["payment"];

            var repository = _container.GetInstance<IDasLevyRepository>();

            var transactions = repository.GetTransactionsByDateRange(accountId, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).Result;

            var paymentTransaction = transactions.OfType<PaymentTransactionLine>().First();

            Assert.AreEqual(accountId, paymentTransaction.AccountId);
            Assert.AreEqual(payment.Amount, paymentTransaction.LineAmount);
            Assert.AreEqual(TransactionItemType.Payment, paymentTransaction.TransactionType);
            Assert.AreEqual(payment.Ukprn, paymentTransaction.UkPrn);
            Assert.AreEqual(payment.CollectionPeriod.Id, paymentTransaction.PeriodEnd);
        }
        
        [Then(@"the apprenticeship course details are stored")]
        public void ThenTheApprenticeshipCourseDetailsAreStored()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var standard = (Standard)ScenarioContext.Current["standard"];
            var apprenticeship = (Apprenticeship)ScenarioContext.Current["apprenticeship"];
            var periodEnd = (PeriodEnd)ScenarioContext.Current["periodEnd"];


            var repository = _container.GetInstance<IDasLevyRepository>();

            var transactions = repository.GetTransactionsByDateRange(accountId, periodEnd.CompletionDateTime.AddDays(-1), periodEnd.CompletionDateTime.AddDays(1)).Result;

            var paymentTransaction = transactions.OfType<PaymentTransactionLine>().First();

            var expectedStartDate = apprenticeship.StartDate?.ToShortDateString() ?? "expectedDateNotFound";
            var courseStartDate = paymentTransaction.CourseStartDate?.ToShortDateString() ?? "courseStartDateNotFound";

            Assert.AreEqual(standard.Level, paymentTransaction.CourseLevel);
            Assert.AreEqual(standard.Title, paymentTransaction.CourseName);
            Assert.AreEqual(expectedStartDate, courseStartDate);
        }
        
        [Then(@"the apprenticeship learner details are stored")]
        public void ThenTheApprenticeshipLearnerDetailsAreStored()
        {
            var accountId = (long)ScenarioContext.Current["AccountId"];
            var apprenticeship = (Apprenticeship)ScenarioContext.Current["apprenticeship"];

            var repository = _container.GetInstance<IDasLevyRepository>();

            var transactions = repository.GetTransactionsByDateRange(accountId, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1)).Result;

            var paymentTransaction = transactions.OfType<PaymentTransactionLine>().First();

            Assert.AreEqual($"{apprenticeship.FirstName} {apprenticeship.LastName}", paymentTransaction.ApprenticeName);
            Assert.AreEqual(apprenticeship.NINumber, paymentTransaction.ApprenticeNINumber);
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
