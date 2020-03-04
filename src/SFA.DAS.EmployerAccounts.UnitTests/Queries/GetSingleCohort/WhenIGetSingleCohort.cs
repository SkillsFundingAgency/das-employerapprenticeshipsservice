using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;
using SFA.DAS.Encoding;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetSingleCohort
{
    public class WhenIGetSingleCohort : QueryBaseTest<GetSingleCohortHandler, GetSingleCohortRequest, GetSingleCohortResponse>
    {
        public override GetSingleCohortRequest Query { get; set; }
        public override GetSingleCohortHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetSingleCohortRequest>> RequestValidator { get; set; }
        private Mock<ICommitmentV2Service> _commitmentV2Service;
        private Mock<IEncodingService> _encodingService;
        private Mock<IHashingService> _hashingService;
        private Mock<ILog> _logger;
        private long _accountId;
        private long _cohortId;
        public string hashedAccountId;        

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountId = 123;
            _cohortId = 1;
            hashedAccountId = "Abc123";
            _logger = new Mock<ILog>();

            _commitmentV2Service = new Mock<ICommitmentV2Service>();
            _commitmentV2Service.Setup(m => m.GetCohorts(_accountId))
                .ReturnsAsync(new List<Cohort>() { new Cohort { Id = _cohortId, NumberOfDraftApprentices = 1 }});

            _commitmentV2Service.Setup(m => m.GetDraftApprenticeships(new Cohort { Id = _cohortId, NumberOfDraftApprentices = 1 }))
                .ReturnsAsync(new List<Apprenticeship> { new Apprenticeship { Id = _cohortId } });

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(hashedAccountId)).Returns(_accountId);

            RequestHandler = new GetSingleCohortHandler(RequestValidator.Object, _logger.Object, _commitmentV2Service.Object, _hashingService.Object);

            Query = new GetSingleCohortRequest
            {
                HashedAccountId = hashedAccountId
            };
        }

       
        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert            
            Assert.IsNotNull(response.Cohort);
            //Assert.IsTrue(response.CohortV2?.Apprenticeships.Count().Equals(1));
        }


        [Test]
        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _commitmentV2Service.Verify(x => x.GetCohorts(_accountId), Times.Once);
        }

        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            return Task.CompletedTask;
        }
    }
}
