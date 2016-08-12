using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries
{
    public abstract class QueryBaseTest<THandler, TRequest, TResponse> 
        where THandler : IAsyncRequestHandler<TRequest, TResponse> 
        where TRequest : IAsyncRequest<TResponse>
    {
        public abstract TRequest Query { get; set; }
        public abstract THandler RequestHandler { get; set; }

        public abstract Mock<IValidator<TRequest>> RequestValidator { get; set; }

        protected void SetUp()
        {
            RequestValidator = new Mock<IValidator<TRequest>>();
            RequestValidator.Setup(x => x.Validate(It.IsAny<TRequest>())).Returns(new ValidationResult());
        }

        [Test]
        public abstract Task ThenIfTheMessageIsValidTheRepositoryIsCalled();

        public abstract Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse();

        [Test]
        public async Task ThenTheReturnValueIsAssignableToTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsAssignableFrom<TResponse>(actual);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            RequestValidator.Verify(x => x.Validate(It.IsAny<TRequest>()), Times.Once);
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheRequestIsNotValid()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<TRequest>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await RequestHandler.Handle(Query));
        }

    }
}