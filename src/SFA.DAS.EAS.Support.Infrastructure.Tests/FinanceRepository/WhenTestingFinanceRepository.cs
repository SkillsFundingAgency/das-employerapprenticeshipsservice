using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Support.Infrastructure.Services;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.FinanceRepository;

public abstract class WhenTestingFinanceRepository
{
    protected Mock<IEmployerFinanceApiService> FinanceService;

    protected IFinanceRepository? Sut;

    [SetUp]
    public void Setup()
    {
        FinanceService = new Mock<IEmployerFinanceApiService>();
        
        Sut = new Services.FinanceRepository(
            FinanceService.Object,
            Mock.Of<ILogger<Services.FinanceRepository>>()
        );
    }
}