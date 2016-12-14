using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeInUse;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerEnglishFractionHistoryTests
{
    public class WhenIGetEnglishFractions : QueryBaseTest<GetEmployerEnglishFractionHandler, GetEmployerEnglishFractionQuery, GetEmployerEnglishFractionResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;
        private Mock<IMediator> _mediator;
        public override GetEmployerEnglishFractionQuery Query { get; set; }
        public override GetEmployerEnglishFractionHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerEnglishFractionQuery>> RequestValidator { get; set; }

        private const string ExpectedEmpRef = "123/ABC";
        private readonly DateTime _expectedAddedDate = new DateTime(2016,02,02);
        private const decimal ExpectedFraction = 0.94m;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetEnglishFractionHistory(ExpectedEmpRef)).ReturnsAsync(new List<DasEnglishFraction> {new DasEnglishFraction {Amount=ExpectedFraction} });

            Query = new GetEmployerEnglishFractionQuery {EmpRef = ExpectedEmpRef};

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPayeSchemeInUseQuery>())).ReturnsAsync(new GetPayeSchemeInUseResponse {PayeScheme = new Scheme {AddedDate = _expectedAddedDate ,Ref = ExpectedEmpRef} });

            RequestHandler = new GetEmployerEnglishFractionHandler(RequestValidator.Object, _dasLevyService.Object, _mediator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerEnglishFractionQuery>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _dasLevyService.Verify(x => x.GetEnglishFractionHistory(ExpectedEmpRef));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerEnglishFractionQuery>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual.Fractions);
            Assert.AreEqual(ExpectedEmpRef,actual.EmpRef);
        }

        [Test]
        public void ThenAnUnauthorizedExceptionIsThrownIfTheValidationResultIsUnauthorized()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerEnglishFractionQuery>())).ReturnsAsync(new ValidationResult
                {
                    IsUnauthorized = true,
                    ValidationDictionary = new Dictionary<string, string>()
                });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(Query));
        }

        [Test]
        public async Task ThenTheSchemeInformationIsCalledForThePassedSchemeRef()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerEnglishFractionQuery>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetPayeSchemeInUseQuery>(c=>c.Empref.Equals(ExpectedEmpRef))));
        }

        [Test]
        public async Task ThenTheSchemeInformationWillBeAddedToTheResponse()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerEnglishFractionQuery>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(actual.EmpRefAddedDate,_expectedAddedDate);
        }
        
    }
}
