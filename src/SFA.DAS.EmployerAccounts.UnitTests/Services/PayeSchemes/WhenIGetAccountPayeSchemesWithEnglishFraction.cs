using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Finance;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Finance;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Encoding;
using OuterApiDasEnglishFraction = SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Responses.Finance.DasEnglishFraction;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.PayeSchemes;

class WhenIGetAccountPayeSchemesWithEnglishFraction
{
    private const long AccountId = 2;
    private static readonly DateTime UpdateDate = DateTime.Now;

    private PayeView _payeView;
    private OuterApiDasEnglishFraction _englishFraction;

    private Mock<IOuterApiClient> _outerApiClient;
    private Mock<IPayeRepository> _payeSchemesRepository;
    private Mock<IEncodingService> _encodingService;
    private IMapper _mapper;

    private IPayeSchemesWithEnglishFractionService _sut;

    [SetUp]
    public void Arrange()
    {
        _payeView = new PayeView
        {
            AccountId = AccountId,
            Ref = "123/ABC"
        };

        _englishFraction = new OuterApiDasEnglishFraction
        {
            EmpRef = _payeView.Ref,
            DateCalculated = UpdateDate,
            Amount = 0.5m
        };

        _outerApiClient = new Mock<IOuterApiClient>();
        _payeSchemesRepository = new Mock<IPayeRepository>();
        _encodingService = new Mock<IEncodingService>();
            
        _mapper = new Mapper(new MapperConfiguration(c =>
        {
            c.AddProfile<LevyMappings>();
        }));

        _payeSchemesRepository.Setup(x => x.GetPayeSchemesByAccountId(It.IsAny<long>())).ReturnsAsync(new List<PayeView>
        {
            _payeView
        });

        _outerApiClient
            .Setup(s => s.Get<GetEnglishFractionCurrentResponse>(It.IsAny<GetEnglishFractionCurrentRequest>()))
            .ReturnsAsync(new GetEnglishFractionCurrentResponse { Fractions = new List<OuterApiDasEnglishFraction> { _englishFraction } });

        _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.AccountId))
            .Returns(AccountId);

        _sut = new PayeSchemesWithEnglishFractionService(
            _outerApiClient.Object,
            _payeSchemesRepository.Object,
            _encodingService.Object,
            _mapper);
    }

    [Test]
    public  async Task ThenIfAccountIdIsValidThenOuterApiIsCalled()
    {
        await _sut.GetPayeSchemes(AccountId);

        _outerApiClient.Verify(v => v.Get<GetEnglishFractionCurrentResponse>(It.IsAny<GetEnglishFractionCurrentRequest>()), Times.Once);
    }

    [Test]
    public  async Task ThenIfAccountIdIsValidPayeSchemesAreReturned()
    {
        //Act
        var result = await _sut.GetPayeSchemes(AccountId);

        //Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(_payeView, result.First());
        Assert.True(CompareEnglishFractions(_englishFraction, result.First().EnglishFraction));
    }

    [Test]
    public async Task ThenIfNotSchemesCanBeFoundNoEnglishFractionsAreCollected()
    {
        _payeSchemesRepository.Setup(x => x.GetPayeSchemesByAccountId(It.IsAny<long>()))
            .ReturnsAsync(new List<PayeView>());

        var result = await _sut.GetPayeSchemes(AccountId);

        //Assert
        Assert.IsEmpty(result);
        _outerApiClient.Verify(v => v.Get<GetEnglishFractionCurrentResponse>(It.IsAny<GetEnglishFractionCurrentRequest>()), Times.Never);
    }

    private static bool CompareEnglishFractions(OuterApiDasEnglishFraction first, EmployerAccounts.Models.Levy.DasEnglishFraction second)
    {
        return JsonConvert.SerializeObject(first).Equals(JsonConvert.SerializeObject(second));
    }
        
}