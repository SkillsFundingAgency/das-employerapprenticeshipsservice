using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using SFA.DAS.EmployerAccounts.Queries.GetAccountCohort;
using SFA.DAS.Encoding;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountCohorts
{
    public class WhenIGetAccountCohorts : QueryBaseTest<GetAccountCohortHandler, GetAccountCohortRequest, GetAccountCohortResponse>
    {
        public override GetAccountCohortRequest Query { get; set; }
        public override GetAccountCohortHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountCohortRequest>> RequestValidator { get; set; }
        private Mock<ICommitmentV2Service> _commitmentV2Service;
        private Mock<IEncodingService> _encodingService;
        private Mock<IHashingService> _hashingService;
        private Mock<ILog> _logger;
        private long _accountId;
        public string hashedAccountId;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountId = 123;
            hashedAccountId = "Abc123";
            _logger = new Mock<ILog>();

            _commitmentV2Service = new Mock<ICommitmentV2Service>();
            _commitmentV2Service.Setup(m => m.GetCohortsV2(_accountId))
                .ReturnsAsync(new List<CohortV2>() { 
                    new CohortV2 
                    {
                        CohortId = 1,
                        Apprenticeships = new List<Apprenticeship>()
                        {
                            new Apprenticeship {Id = 2 }
                        }
                    } 
                
                });
                
            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(hashedAccountId)).Returns(_accountId);

            RequestHandler = new GetAccountCohortHandler(RequestValidator.Object, _logger.Object, _commitmentV2Service.Object, _encodingService.Object, _hashingService.Object);

            Query = new GetAccountCohortRequest
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
            Assert.IsNotNull(response.CohortV2);            
        }


        [Test]
        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _commitmentV2Service.Verify(x => x.GetCohortsV2(_accountId), Times.Once);
        }

        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            return Task.CompletedTask;
        }
    }
}
