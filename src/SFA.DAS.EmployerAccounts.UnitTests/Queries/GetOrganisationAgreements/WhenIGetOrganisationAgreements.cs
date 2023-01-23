using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetOrganisationAgreements
{
    public class WhenIGetOrganisationAgreements : QueryBaseTest<GetOrganisationAgreementsQueryHandler, GetOrganisationAgreementsRequest, GetOrganisationAgreementsResponse>
    {
        public override GetOrganisationAgreementsRequest Query { get; set; }
        public override GetOrganisationAgreementsQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetOrganisationAgreementsRequest>> RequestValidator { get; set; }
        private Mock<IEmployerAgreementRepository> _mockEmployerAgreementRepository;
        private Mock<IAccountLegalEntityPublicHashingService> _mockAccountLegalEntityPublicHashingService;
        private Mock<IHashingService> _mockhashingService;
        private Mock<IReferenceDataService> _mockreferenceDataService;
        private Mock<IMapper> _mockmapper;
        private long _accountLegelEntityId = 123;
        private string hashedId = "ABC123";

        [SetUp]
        public void Arrange()
        {
            SetUp();
           
            _mockAccountLegalEntityPublicHashingService = new Mock<IAccountLegalEntityPublicHashingService>();
            _mockAccountLegalEntityPublicHashingService.Setup(m => m.DecodeValue(It.IsAny<string>())).Returns(_accountLegelEntityId);

            _mockEmployerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _mockEmployerAgreementRepository.Setup(m => m.GetOrganisationsAgreements(It.IsAny<long>())).ReturnsAsync(new AccountLegalEntity {       
                 LegalEntity = new LegalEntity { Id=1, Source= OrganisationType.CompaniesHouse } , 
                 Agreements = new List<EmployerAgreement>()  { new EmployerAgreement { SignedDate = DateTime.UtcNow, } }});

            _mockhashingService = new Mock<IHashingService>();
            _mockhashingService.Setup(m => m.HashValue(It.IsAny<long>())).Returns(hashedId);

            _mockreferenceDataService = new Mock<IReferenceDataService>();
            _mockreferenceDataService.Setup(m => m.IsIdentifiableOrganisationType(It.IsAny<OrganisationType>())).ReturnsAsync(true);

            var agreements = new List<EmployerAgreementDto> { 
                new EmployerAgreementDto  { Id =1, SignedDate = DateTime.UtcNow , AccountLegalEntity = new AccountLegalEntityDto { Id = _accountLegelEntityId} },
                new EmployerAgreementDto  { Id =2, SignedDate = DateTime.UtcNow , AccountLegalEntity = new AccountLegalEntityDto { Id = _accountLegelEntityId} }};

            _mockmapper = new Mock<IMapper>();
            _mockmapper.Setup(m => m.Map<ICollection<EmployerAgreement>, ICollection<EmployerAgreementDto>>(It.IsAny<ICollection<EmployerAgreement>>(),
              It.IsAny<Action<IMappingOperationOptions<ICollection<EmployerAgreement>, ICollection<EmployerAgreementDto>>>>()))
          .Returns(agreements);

            RequestHandler = new GetOrganisationAgreementsQueryHandler(RequestValidator.Object,  _mockEmployerAgreementRepository.Object, _mockAccountLegalEntityPublicHashingService.Object, 
                _mockhashingService.Object, _mockreferenceDataService.Object, _mockmapper.Object);

            Query = new GetOrganisationAgreementsRequest
            {
                AccountLegalEntityHashedId = hashedId
            };
        }


        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _mockEmployerAgreementRepository.Verify(x => x.GetOrganisationsAgreements(It.IsAny<long>()), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.IsNotNull(response.Agreements);
            Assert.IsTrue(response.Agreements.Any());
        }

        
    }
}
