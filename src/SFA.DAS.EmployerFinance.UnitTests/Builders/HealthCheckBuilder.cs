using Moq;
using SFA.DAS.EmployerFinance.Models;

namespace SFA.DAS.EmployerFinance.UnitTests.Builders
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