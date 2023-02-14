using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries
{
    public abstract class QueryBaseTest<THandler, TRequest, TResponse>
        where THandler : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public abstract TRequest Query { get; set; }
        public abstract THandler RequestHandler { get; set; }

        public abstract Mock<IValidator<TRequest>> RequestValidator { get; set; }

        private int _validationCallCount;

        protected void SetUp()
        {
            _validationCallCount = 0;
            RequestValidator = new Mock<IValidator<TRequest>>();
            RequestValidator.Setup(x => x.Validate(It.IsAny<TRequest>())).Returns(new ValidationResult()).Callback(() => _validationCallCount++);
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<TRequest>())).ReturnsAsync(new ValidationResult()).Callback(() => _validationCallCount++);
        }

        [Test]
        public abstract Task ThenIfTheMessageIsValidTheRepositoryIsCalled();

        public abstract Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse();

        [Test]
        public async Task ThenTheReturnValueIsAssignableToTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.IsAssignableFrom<TResponse>(actual);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.AreEqual(1, _validationCallCount);
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheRequestIsNotValid()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<TRequest>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<TRequest>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await RequestHandler.Handle(Query, CancellationToken.None));
        }
    }
}