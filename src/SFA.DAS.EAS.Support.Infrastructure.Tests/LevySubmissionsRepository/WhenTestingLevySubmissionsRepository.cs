﻿using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.LevySubmissionsRepository;

[TestFixture]
public class WhenTestingLevySubmissionsRepository
{
    private Mock<IApprenticeshipLevyApiClient>? _levyApiClient;
    private Mock<ILogger<Services.LevySubmissionsRepository>>? _logger;
    private Mock<ILevyTokenHttpClientFactory>? _levyTokenHttpClientFactory;
    private string? _testPayeScheme;
    private LevyDeclarations? _levyDeclarations;

    [SetUp]
    public void Setup()
    {
        _testPayeScheme = "123/EROEROO";

        _levyDeclarations = new LevyDeclarations
        {
            Declarations = new List<Declaration>
            {
                new Declaration
                {
                    Id = "001",
                    SubmissionTime = new DateTime(2017, 3, 1)
                },
                new Declaration
                {
                    Id = "002",
                    SubmissionTime = new DateTime(2017, 4, 1)
                },
                new Declaration
                {
                    Id = "003",
                    SubmissionTime = new DateTime(2017, 4, 1)
                }
            }
        };

        _levyApiClient = new Mock<IApprenticeshipLevyApiClient>();
        _logger = new Mock<ILogger<Services.LevySubmissionsRepository>>();
        _levyTokenHttpClientFactory = new Mock<ILevyTokenHttpClientFactory>();
    }

    [Test]
    public async Task ThenItShouldRequestLevyClientAndGetLevies()
    {
        //Arrange
        _levyApiClient!
            .Setup(x => x.GetEmployerLevyDeclarations(_testPayeScheme, null, null))
            .Returns(Task.FromResult(_levyDeclarations));

        _levyTokenHttpClientFactory!
            .Setup(x => x.GetLevyHttpClient())
            .Returns(Task.FromResult(_levyApiClient.Object));

        //Act
        var sut = new Services.LevySubmissionsRepository(_logger!.Object, _levyTokenHttpClientFactory.Object);
        var result = await sut.Get(_testPayeScheme);

        //Assert
        _levyTokenHttpClientFactory.Verify(x => x.GetLevyHttpClient(), Times.Once);
        _levyApiClient.Verify(x => x.GetEmployerLevyDeclarations(_testPayeScheme, null, null), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Declarations, Is.Not.Null);
    }


    [Test]
    public async Task ThenItShouldNotReturnLevyDeclarationsBeforeApril2017()
    {
        //Arrange
        var mar2017Date = new DateTime(2017, 3, 1);

        _levyApiClient!
            .Setup(x => x.GetEmployerLevyDeclarations(_testPayeScheme, null, null))
            .Returns(Task.FromResult(_levyDeclarations));

        _levyTokenHttpClientFactory!
            .Setup(x => x.GetLevyHttpClient())
            .Returns(Task.FromResult(_levyApiClient.Object));

        //Act
        var sut = new Services.LevySubmissionsRepository(_logger!.Object, _levyTokenHttpClientFactory.Object);
        var result = await sut.Get(_testPayeScheme);

        var levyBeforeApr2017 = result?.Declarations?.Any(x => x.SubmissionTime < mar2017Date);

        //Assert
        Assert.That(levyBeforeApr2017, Is.Not.Null);
        Assert.That(levyBeforeApr2017, Is.False);
    }

}