using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetOrganisationByIdTests
{
    public class WhenISearchForAnOrganisationById
    {
        private Mock<IReferenceDataService> _referenceDataService;
        private Mock<IPensionRegulatorService> _pensionRegulatorService;
        public IConfigurationProvider _ConfigurationProvider;
        public GetOrganisationByIdQueryHandler _requestHandler;
        public Mock<IValidator<GetOrganisationByIdRequest>> _requestValidator;

        [SetUp]
        public void Arrange()
        {
            _referenceDataService = new Mock<IReferenceDataService>();
            _pensionRegulatorService = new Mock<IPensionRegulatorService>();
            _requestValidator = new Mock<IValidator<GetOrganisationByIdRequest>>();
            _requestValidator.Setup(x => x.Validate(It.IsAny<GetOrganisationByIdRequest>())).Returns(new ValidationResult());
            _ConfigurationProvider = new MapperConfiguration(c => { c.AddProfile<ReferenceDataMappings>(); });

            _requestHandler = new GetOrganisationByIdQueryHandler(_requestValidator.Object, _referenceDataService.Object, _pensionRegulatorService.Object, _ConfigurationProvider.CreateMapper());
        }

        [TestCase(OrganisationType.PensionsRegulator)]
        [TestCase(OrganisationType.CompaniesHouse)]
        public async Task ThenIfTheCorrectLookupServiceIsCalled(OrganisationType organisationType)
        {
            //Arrange
            var expectedIdentifier = "123xyz";

            //Act
            await _requestHandler.Handle(new GetOrganisationByIdRequest {Identifier = expectedIdentifier, OrganisationType = organisationType }, CancellationToken.None);

            //Assert
            if (organisationType == OrganisationType.PensionsRegulator)
            {
                _referenceDataService.Verify(x => x.GetLatestDetails(organisationType, expectedIdentifier), Times.Never);
                _pensionRegulatorService.Verify(x => x.GetOrganisationById(expectedIdentifier), Times.Once);
            }
            else
            {
                _referenceDataService.Verify(x => x.GetLatestDetails(organisationType, expectedIdentifier), Times.Once);
                _pensionRegulatorService.Verify(x => x.GetOrganisationById(expectedIdentifier), Times.Never);
            }
        }
        
        [TestCase("12345678", OrganisationType.PensionsRegulator)]
        [TestCase("XYZ", OrganisationType.CompaniesHouse)]
        public async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse(string id, OrganisationType organisationType)
        {
            var _query = new GetOrganisationByIdRequest
            {
                Identifier = id,
                OrganisationType = organisationType
            };

            //Arrange
            _pensionRegulatorService.Setup(x => x.GetOrganisationById(_query.Identifier)).ReturnsAsync(new Organisation {Name = id});
            _referenceDataService.Setup(x => x.GetLatestDetails(_query.OrganisationType, _query.Identifier)).ReturnsAsync(new ReferenceData.Types.DTO.Organisation {Name = id});

            //Act
            var actual = await _requestHandler.Handle(_query, CancellationToken.None);

            //Assert
            Assert.AreSame(id, actual.Organisation.Name);
        }
    }
}
