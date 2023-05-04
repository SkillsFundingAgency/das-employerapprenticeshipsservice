using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountEmployerAgreements
{
    [TestFixture]
    public class GetAccountEmployerAgreementsQueryHandlerTests
    {
        private Mock<IEncodingService> _mockEncodingService;
        private Mock<IValidator<GetAccountEmployerAgreementsRequest>> _mockValidator;
        private Fixture _fixture;
        public Mock<EmployerAccountsDbContext> _dbMock;
        public List<AccountLegalEntity> _accountLegalEntities;

        private GetAccountEmployerAgreementsQueryHandler _handler;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mockEncodingService = new Mock<IEncodingService>();
            _mockValidator = new Mock<IValidator<GetAccountEmployerAgreementsRequest>>();

            _fixture = new Fixture();
            _fixture
                .Customize(new AutoMoqCustomization())
                .Behaviors.Add(new OmitOnRecursionBehavior());

            _dbMock = new Mock<EmployerAccountsDbContext>();
            _accountLegalEntities = new List<AccountLegalEntity>();

            var mockDbSet = _accountLegalEntities.AsQueryable().BuildMockDbSet();

            _dbMock.Setup(d => d.AccountLegalEntities).Returns(mockDbSet.Object);
            MapperConfiguration configuration = ApplyMappings();

            _mapper = configuration.CreateMapper();

            _handler = new GetAccountEmployerAgreementsQueryHandler(
                new Lazy<EmployerAccountsDbContext>(() => _dbMock.Object),
                _mockEncodingService.Object,
                _mockValidator.Object,
                _mapper.ConfigurationProvider);
        }

        [Test]
        public async Task Handle_WhenRequestIsValid_ReturnsResponseWithAgreements()
        {
            // Arrange
            var request = _fixture.Create<GetAccountEmployerAgreementsRequest>();

            _mockValidator.Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult());

            _mockEncodingService.Setup(x => x.Encode(request.AccountId, EncodingType.AccountId))
                .Returns<long, EncodingType>((id, et) => $"encoded_{id}");

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
        }

        [Test]
        public async Task Handle_WhenSingleAccountLegalEntityWithPendingAgreement_ThenHasPendingAgreement()
        {
            // Arrange
            var request = _fixture.Create<GetAccountEmployerAgreementsRequest>();

            var accountLegalEntity = _fixture
                .Build<AccountLegalEntity>()
                .With(ale => ale.AccountId, request.AccountId)
                .Without(ale => ale.SignedAgreement)
                .Without(ale => ale.SignedAgreementId)
                .Without(ale => ale.SignedAgreementVersion)
                .Without(ale => ale.Deleted)
                .Create();

            _accountLegalEntities.Add(accountLegalEntity);

            _mockValidator.Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult());

            _mockEncodingService.Setup(x => x.Encode(request.AccountId, EncodingType.AccountId))
                .Returns<long, EncodingType>((id, et) => $"encoded_{id}");

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.HasPendingAgreements.Should().BeTrue();
        }

        [Test]
        public async Task Handle_WhenSingleAccountLegalEntityWithSignedAndPendingAgreement_ThenHasPendingAgreement()
        {
            // Arrange
            var request = _fixture.Create<GetAccountEmployerAgreementsRequest>();

            var signedTemplate = _fixture
                .Build<AgreementTemplate>()
                .With(at => at.VersionNumber, 2)
                .Create();

            var pendingTemplate = _fixture
                .Build<AgreementTemplate>()
                .With(at => at.VersionNumber, 5)
                .Create();

            var accountLegalEntity = _fixture
                .Build<AccountLegalEntity>()
                .With(ale => ale.AccountId, request.AccountId)
                .With(ale => ale.SignedAgreement, _fixture.Build<EmployerAgreement>().With(ea => ea.Template, signedTemplate).Create())
                .With(ale => ale.PendingAgreement, _fixture.Build<EmployerAgreement>().With(ea => ea.Template, pendingTemplate).Create())
                .Without(ale => ale.Deleted)
                .Create();

            _accountLegalEntities.Add(accountLegalEntity);

            _mockValidator.Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult());

            _mockEncodingService.Setup(x => x.Encode(request.AccountId, EncodingType.AccountId))
                .Returns<long, EncodingType>((id, et) => $"encoded_{id}");

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.HasPendingAgreements.Should().BeTrue();
        }

        [Test]
        public async Task Handle_WhenMultipleAccountLegalEntityWithSignedAgreements_ThenMinimumSignedAgreementSet()
        {
            // Arrange
            var request = _fixture.Create<GetAccountEmployerAgreementsRequest>();
            int[] signedAgreementVersions = new[] { 2, 3, 7 };

            var accountLegalEntities = _fixture
                .Build<AccountLegalEntity>()
                .With(ale => ale.AccountId, request.AccountId)
                .Without(ale => ale.Deleted)
                .CreateMany()
                .ToList();

            accountLegalEntities = accountLegalEntities
                .Zip(signedAgreementVersions, (ale, version) =>
                {
                    ale.SignedAgreementVersion = version;
                    ale.SignedAgreement.Template.VersionNumber = version;
                    return ale;
                })
                .ToList();

            _accountLegalEntities.AddRange(accountLegalEntities);

            _mockValidator.Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult());

            _mockEncodingService.Setup(x => x.Encode(request.AccountId, EncodingType.AccountId))
                .Returns<long, EncodingType>((id, et) => $"encoded_{id}");

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.MinimumSignedAgreementVersion.Should().Be(signedAgreementVersions.Min());
        }

        [Test]
        public void Handle_WhenRequestIsInvalid_ThrowsInvalidRequestException()
        {
            // Arrange
            var request = _fixture.Create<GetAccountEmployerAgreementsRequest>();

            var validationResult = new ValidationResult();
            validationResult.AddError("AccountId", "AccountId error");

            _mockValidator.Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(validationResult);

            // Act

            // Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(request, CancellationToken.None));
        }

        private static MapperConfiguration ApplyMappings()
        {

            // Create a test MapperConfiguration
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AccountLegalEntity, EmployerAgreementStatusDto>()
                .ForMember(d => d.LegalEntity, o => o.MapFrom(g => g.LegalEntity))
                .ForMember(d => d.AccountId, o => o.Ignore())
                .ForMember(d => d.HashedAccountId, o => o.Ignore())
                .ForMember(d => d.Signed, o => o.MapFrom(g => g.SignedAgreement))
                .ForMember(d => d.Pending, o => o.MapFrom(g => g.PendingAgreement))
                .ForMember(d => d.LegalEntity, o => o.MapFrom(l => l));

                cfg.CreateMap<EmployerAgreement, SignedEmployerAgreementDetailsDto>()
                .ForMember(d => d.HashedAgreementId, o => o.Ignore())
                .ForMember(d => d.PartialViewName, conf => conf.MapFrom(ol => ol.Template.PartialViewName))
                .ForMember(d => d.TemplateId, conf => conf.MapFrom(ol => ol.Template.Id))
                .ForMember(d => d.VersionNumber, conf => conf.MapFrom(ol => ol.Template.VersionNumber));

                cfg.CreateMap<AccountLegalEntity, AccountSpecificLegalEntityDto>()
                .ForMember(d => d.RegisteredAddress, o => o.MapFrom(l => l.Address))
                .ForMember(d => d.Id, o => o.MapFrom(l => l.LegalEntityId))
                .ForMember(d => d.DateOfIncorporation, o => o.MapFrom(l => l.LegalEntity.DateOfIncorporation))
                .ForMember(d => d.Code, o => o.MapFrom(l => l.LegalEntity.Code))
                .ForMember(d => d.Sector, o => o.MapFrom(l => l.LegalEntity.Sector))
                .ForMember(d => d.Status, o => o.MapFrom(l => l.LegalEntity.Status))
                .ForMember(d => d.AccountLegalEntityId, o => o.MapFrom(l => l.Id))
                .ForMember(d => d.PublicSectorDataSource, o => o.MapFrom(l => l.LegalEntity.PublicSectorDataSource))
                .ForMember(d => d.Source, o => o.MapFrom(l => l.LegalEntity.Source))
                .ForMember(d => d.AccountLegalEntityId, o => o.MapFrom(l => l.Id))
                .ForMember(d => d.AccountLegalEntityPublicHashedId, o => o.MapFrom(l => l.PublicHashedId));

                cfg.CreateMap<EmployerAgreement, EmployerAgreementDetailsDto>()
                .ForMember(d => d.HashedAgreementId, o => o.Ignore())
                .ForMember(d => d.PartialViewName, conf => conf.MapFrom(ol => ol.Template.PartialViewName))
                .ForMember(d => d.TemplateId, conf => conf.MapFrom(ol => ol.Template.Id))
                .ForMember(d => d.VersionNumber, conf => conf.MapFrom(ol => ol.Template.VersionNumber));
            });
        }
    }

}
