using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Moq;
using NUnit.Framework;
using SFA.DAS.Caches;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.PaymentServiceTests
{
    internal class WhenIGetAccountPayments
    {
        private const string PeriodEnd = "R12-13";
        private const long AccountId = 2;
        private const string StandardCourseName = "Standard Course";
        private const string FrameworkCourseName = "Framework Course";

        private Mock<IApprenticeshipInfoServiceWrapper> _apprenticeshipInfoService;
        private Mock<ICommitmentsV2ApiClient> _commitmentsV2ApiClient;
        private Mock<IPaymentsEventsApiClient> _paymentsApiClient;
        private Mock<IMapper> _mapper;
        private Mock<ILog> _logger;
        private Mock<IInProcessCache> _cacheProvider;
        private Mock<IProviderService> _providerService;

        private PaymentService _paymentService;
        private Framework _framework;
        private Standard _standard;
        private GetApprenticeshipResponse _apprenticeship;
        private EmployerFinance.Models.ApprenticeshipProvider.Provider _provider;
        private PaymentDetails _standardPayment;
        private PaymentDetails _frameworkPayment;

        [SetUp]
        public void Arrange()
        {
            SetupTestModels();
            SetupPaymentsApiMock();
            SetupCommitmentsApiMock();
            SetupApprenticeshipServiceMock();
            SetupMapperMock();
            SetupLoggerMock();
            SetupProviderServiceMock();

            _cacheProvider = new Mock<IInProcessCache>();

            _paymentService = new PaymentService(
                _paymentsApiClient.Object,
                _commitmentsV2ApiClient.Object,
                _apprenticeshipInfoService.Object,
                _mapper.Object,
                _logger.Object,
                _cacheProvider.Object,
                _providerService.Object);
        }

        [Test]
        public async Task ThenThePaymentsApiIsCalledToGetPaymentData()
        {
            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _paymentsApiClient.Verify(x => x.GetPayments(PeriodEnd, AccountId.ToString(), 1, null));
        }

        [Test]
        public async Task ThenTheCommitmentsApiIsCalledToGetApprenticeDetails()
        {
            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _commitmentsV2ApiClient.Verify(x => x.GetApprenticeship(_apprenticeship.Id), Times.Once);
        }


        [Test]
        public async Task ThenTheProviderServiceIsCalledToGetTheProvider()
        {

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _providerService.Verify(x => x.Get(_provider.Ukprn), Times.Once);
        }        

        [Test]
        public async Task ThenTheAppreticeshipsApiIsCalledToGetStandardDetails()
        {
            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Once);
            _apprenticeshipInfoService.Verify(x => x.GetFrameworksAsync(false), Times.Never);
        }

        [Test]
        public async Task ThenSubsequentCallsToGetStandardDetailsAreReadFromTheCache()
        {
            //Arrange
            _cacheProvider.SetupSequence(
                x => x.Get<StandardsView>(nameof(StandardsView)))
                .Returns((StandardsView)null)
                .Returns(new StandardsView { Standards = new List<Standard>() });


            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Once);
            _cacheProvider.Verify(x => x.Get<StandardsView>(nameof(StandardsView)), Times.Exactly(2));
        }

        [Test]
        public async Task ThenTheAppreticeshipsApiIsCalledToGetFrameworkDetails()
        {
            //Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                     .Returns(() => _frameworkPayment);

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetFrameworksAsync(false), Times.Once);
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Never);
        }

        [Test]
        public async Task ThenSubsequentCallsToGetFrameworksAreReadFromTheCache()
        {
            //Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                     .Returns(() => _frameworkPayment);
            _cacheProvider.SetupSequence(
                x => x.Get<FrameworksView>(nameof(FrameworksView)))
                .Returns((FrameworksView)null)
                .Returns(new FrameworksView { Frameworks = new List<Framework>() });

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetFrameworksAsync(false), Times.Once);
            _cacheProvider.Verify(x => x.Get<FrameworksView>(nameof(FrameworksView)), Times.Exactly(2));
        }

        [Test]
        public async Task ThenIShouldGetPaymentDetails()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            Assert.AreEqual(AccountId, details.First().EmployerAccountId);
        }

        [Test]
        public async Task ThenIShouldGetCorrectPeriodEnd()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            Assert.AreEqual(PeriodEnd, details.First().PeriodEnd);
        }

        [Test]
        public async Task ThenIShouldGetCorrectProviderName()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            Assert.AreEqual(_provider.Name, details.First().ProviderName);
        }

        [Test]
        public async Task ThenIShouldGetCorrectStandardCourseName()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            Assert.AreEqual(StandardCourseName, details.First().CourseName);
        }

        [Test]
        public async Task ThenIShouldGetCorrectFrameworkCourseName()
        {
            //Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                    .Returns(() => _frameworkPayment);

            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            Assert.AreEqual(FrameworkCourseName, details.First().CourseName);
        }

        [Test]
        public async Task ThenIShouldGetCorrectFrameworkPathwayName()
        {
            //Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                .Returns(() => _frameworkPayment);

            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            Assert.AreEqual(_framework.PathwayName, details.First().PathwayName);
        }

        [Test]
        public async Task ThenShouldNotAttemptToGetStandardWithNullStandardCode()
        {
            // Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                .Returns(() => _frameworkPayment);

            // Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            // Assert
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Never);
        }

        [Test]
        public async Task ThenShouldNotAttemptToGetStandardWithZeroStandardCode()
        {
            // Arrange
            _frameworkPayment.StandardCode = 0;
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                .Returns(() => _frameworkPayment);

            // Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            // Assert
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Never);
        }

        [Test]
        public async Task ThenShouldNotAttemptToGetFrameworkWithNullFrameworkCode()
        {
            // Arrange
            _frameworkPayment.FrameworkCode = null;
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                .Returns(() => _frameworkPayment);

            // Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            // Assert
            _apprenticeshipInfoService.Verify(x => x.GetFrameworksAsync(false), Times.Never);
        }

        [Test]
        public async Task ThenShouldNotAttemptToGetFrameworkWithZeroFrameworkCode()
        {
            // Arrange
            _frameworkPayment.FrameworkCode = 0;
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                .Returns(() => _frameworkPayment);

            // Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            // Assert
            _apprenticeshipInfoService.Verify(x => x.GetFrameworksAsync(false), Times.Never);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, null)]
        [TestCase(null, 0)]
        [TestCase(null, null)]
        public async Task ThenShouldLogWarningIfBothStandardCodeAndFramworkCodeNotSet(int? invalidFrameworkCode, int? invalidStandardCode)
        {
            // Arrange
            _frameworkPayment.FrameworkCode = invalidFrameworkCode;
            _frameworkPayment.StandardCode = invalidStandardCode;
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                .Returns(() => _frameworkPayment);

            // Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            // Assert
            _logger.Verify(x => x.Warn(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenIShouldGetCorrectApprenticeDetails()
        {
            //Act
            var details = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            var apprenticeName = $"{_apprenticeship.FirstName} {_apprenticeship.LastName}";

            Assert.AreEqual(apprenticeName, details.First().ApprenticeName);
        }

        [Test]
        public async Task ThenShouldLogWarningIfCommitmentsApiCallFails()
        {
            //Arrange
            _commitmentsV2ApiClient.Setup(x => x.GetApprenticeship(It.IsAny<long>()))
                .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<Exception>(),
                $"Unable to get Apprenticeship with Employer Account ID {AccountId} and " +
                $"apprenticeship ID {_standardPayment.ApprenticeshipId} from commitments API."), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogErrorIfPaymentsApiCallFails()
        {
            //Arrange
            _paymentsApiClient.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), null))
                              .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _logger.Verify(x => x.Error(It.IsAny<Exception>(),
                $"Unable to get payment information for {PeriodEnd} accountid {AccountId}"), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogWarningIfApprenticeshipsApiCallFailsWhenGettingStandards()
        {
            //Arrange
            _apprenticeshipInfoService.Setup(x => x.GetStandardsAsync(It.IsAny<bool>()))
                                      .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<Exception>(), "Could not get standards from apprenticeship API."), Times.Once);
        }

        [Test]
        public async Task ThenShouldLogWarningIfApprenticeshipsApiCallFailsWhenGettingFrameworks()
        {
            //Arrange
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                  .Returns(() => _frameworkPayment);
            _apprenticeshipInfoService.Setup(x => x.GetFrameworksAsync(It.IsAny<bool>()))
                                      .Throws<WebException>();

            //Act
            await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _logger.Verify(x => x.Warn(It.IsAny<Exception>(), "Could not get frameworks from apprenticeship API."), Times.Once);
        }

        [Test]
        public async Task ThenShouldGetPaymentsFromAllPages()
        {
            //Arrange
            const int numberOfPages = 3;

            _paymentsApiClient.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), null))
                .ReturnsAsync(new PageOfResults<Provider.Events.Api.Types.Payment>
                {
                    Items = new[] { new Provider.Events.Api.Types.Payment(), new Provider.Events.Api.Types.Payment() },
                    TotalNumberOfPages = numberOfPages
                });

            //Act
            var result = await _paymentService.GetAccountPayments(PeriodEnd, AccountId, Guid.NewGuid());

            //Assert
            _paymentsApiClient.Verify(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), null), Times.Exactly(numberOfPages));
            _paymentsApiClient.Verify(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), 1, null), Times.Once);
            _paymentsApiClient.Verify(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), 2, null), Times.Once);
            _paymentsApiClient.Verify(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), 3, null), Times.Once);

            Assert.AreEqual(2 * numberOfPages, result.Count);

        }

        private void SetupLoggerMock()
        {
            _logger = new Mock<ILog>();
            _logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            _logger.Setup(x => x.Warn(It.IsAny<Exception>(), It.IsAny<string>()));
        }

        private void SetupMapperMock()
        {
            _mapper = new Mock<IMapper>();
            _mapper.Setup(x => x.Map<PaymentDetails>(It.IsAny<Provider.Events.Api.Types.Payment>()))
                .Returns(() => _standardPayment);
        }

        private void SetupProviderServiceMock()
        {
            _providerService = new Mock<IProviderService>();
            _providerService.Setup(x => x.Get(It.IsAny<long>()))
                .ReturnsAsync(_provider);
        }

        private void SetupTestModels()
        {
            _framework = new Framework
            {
                FrameworkName = FrameworkCourseName,
                FrameworkCode = 20,
                PathwayCode = 2,
                PathwayName = "General",
                ProgrammeType = 3
            };

            _standard = new Standard
            {
                Id = "10",
                Code = 10,
                CourseName = StandardCourseName
            };

            _apprenticeship = new GetApprenticeshipResponse
            {
                Id = 545646,
                FirstName = "John",
                LastName = "Doe",
                NINumber = "12345678"
            };

            _provider = new EmployerFinance.Models.ApprenticeshipProvider.Provider
            {
                Id = 10,
                Ukprn = 74765,
                Name = "Test Provider"
            };

            _standardPayment = new PaymentDetails
            {
                Id = Guid.NewGuid(),
                EmployerAccountId = AccountId,
                Ukprn = _provider.Ukprn,
                ApprenticeshipId = _apprenticeship.Id,
                StandardCode = _standard.Code,

            };

            _frameworkPayment = new PaymentDetails
            {
                Id = Guid.NewGuid(),
                EmployerAccountId = AccountId,
                Ukprn = _provider.Ukprn,
                ApprenticeshipId = _apprenticeship.Id,
                FrameworkCode = _framework.FrameworkCode,
                PathwayCode = _framework.PathwayCode,
                ProgrammeType = _framework.ProgrammeType
            };
        }

        private void SetupPaymentsApiMock()
        {
            _paymentsApiClient = new Mock<IPaymentsEventsApiClient>();
            _paymentsApiClient.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), null))
                .ReturnsAsync(new PageOfResults<Provider.Events.Api.Types.Payment>
                {
                    Items = new[] { new Provider.Events.Api.Types.Payment() }
                });
        }

        private void SetupCommitmentsApiMock()
        {
            _commitmentsV2ApiClient = new Mock<ICommitmentsV2ApiClient>();
            _commitmentsV2ApiClient.Setup(x => x.GetApprenticeship(It.IsAny<long>()))
                .ReturnsAsync(_apprenticeship);
        }

        private void SetupApprenticeshipServiceMock()
        {
            _apprenticeshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();

            _apprenticeshipInfoService.Setup(x => x.GetFrameworksAsync(false))
                .ReturnsAsync(new FrameworksView
                {
                    Frameworks = new EditableList<Framework>
                    {
                        _framework
                    }
                });

            _apprenticeshipInfoService.Setup(x => x.GetStandardsAsync(false))
                .ReturnsAsync(new StandardsView
                {
                    Standards = new EditableList<Standard>
                    {
                        _standard
                    }
                });
        }
    }
}