using Moq;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.UnitTests.Builders
{
    public class HealthCheckBuilder
    {
        private readonly Mock<HealthCheck> _healthCheck = new Mock<HealthCheck> { CallBase = true };

        public HealthCheckBuilder WithId(int id)
        {
            _healthCheck.Setup(h => h.Id).Returns(id);

            return this;
        }

        public HealthCheck Build()
        {
            return _healthCheck.Object;
        }
    }
}