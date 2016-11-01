using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdateEnglishFractions;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpdateEnglishFractions
{
    class WhenISendTheUpdateEnglishFractionsCommand
    {
        private UpdateEnglishFractionsCommandHandler _handler;
        private Mock<IHmrcService> _hmrcService;
        private Mock<IEnglishFractionRepository> _englishFractionRepository;
        private Mock<ILogger> _logger;
        private List<DasEnglishFraction> _existingFractions;
        private string _employerReference;
        private List<FractionCalculation> _fractionCalculations;

        [SetUp]
        public void Arrange()
        {
            _employerReference = "123/AB456";
            _englishFractionRepository = new Mock<IEnglishFractionRepository>();
            _hmrcService = new Mock<IHmrcService>();
            _logger = new Mock<ILogger>();
            _handler = new UpdateEnglishFractionsCommandHandler(_hmrcService.Object, _englishFractionRepository.Object, _logger.Object);

            _existingFractions = new List<DasEnglishFraction>
            {
                new DasEnglishFraction
                {
                    Id = "1",
                    DateCalculated = DateTime.Parse(DateTime.Now.AddDays(-20).ToShortDateString()),
                    EmpRef = _employerReference,
                    Amount = 0.45M
                },
                new DasEnglishFraction
                {
                    Id = "2",
                    DateCalculated = DateTime.Parse(DateTime.Now.AddDays(-10).ToShortDateString()),
                    EmpRef = _employerReference,
                    Amount = 0.5M
                }
            };

            _fractionCalculations = new List<FractionCalculation>
            {
                new FractionCalculation
                {
                    CalculatedAt = DateTime.Now.AddDays(-20).ToShortDateString(),
                    Fractions = new List<Fraction>
                    {
                        new Fraction {Region = "England", Value = "0.45"}
                    }
                },
                new FractionCalculation
                {
                    CalculatedAt = DateTime.Now.AddDays(-10).ToShortDateString(),
                    Fractions = new List<Fraction>
                    {
                        new Fraction {Region = "England", Value = "0.5"}
                    }
                },
                new FractionCalculation
                {
                    CalculatedAt = DateTime.Now.AddDays(-5).ToShortDateString(),
                    Fractions = new List<Fraction>
                    {
                        new Fraction {Region = "England", Value = "0.55"}
                    }
                },
                new FractionCalculation
                {
                    CalculatedAt = DateTime.Now.ToShortDateString(),
                    Fractions = new List<Fraction>
                    {
                        new Fraction {Region = "England", Value = "0.6"}
                    }
                }
            };
        }

        [Test]
        public async Task ThenIShouldUpdateEnglishFractions()
        {
            //Assign
            _englishFractionRepository.Setup(
                x => x.CreateEmployerFraction(It.IsAny<DasEnglishFraction>(), It.IsAny<string>()))
                .Returns(Task.Run(() => { }));

            _englishFractionRepository.Setup(x => x.GetAllEmployerFractions(_employerReference))
                .ReturnsAsync(_existingFractions);

            _hmrcService.Setup(x => x.GetEnglishFractions(It.IsAny<string>(), _employerReference))
                .ReturnsAsync(new EnglishFractionDeclarations
                {
                    Empref = _employerReference,
                    FractionCalculations = _fractionCalculations
                });

            //Act
            await _handler.Handle(new UpdateEnglishFractionsCommand
            {
                EmployerReference = _employerReference
            });

            //Assert
            _englishFractionRepository.Verify(x => x.GetAllEmployerFractions(_employerReference), Times.Once);
            _hmrcService.Verify(x => x.GetEnglishFractions(It.IsAny<string>(), _employerReference), Times.Once);

            _englishFractionRepository.Verify(x => x.CreateEmployerFraction(
                It.Is<DasEnglishFraction>(fraction => IsSameAsFractionCalculation(fraction, _fractionCalculations[2])), 
                _employerReference), Times.Once);

            _englishFractionRepository.Verify(x => x.CreateEmployerFraction(
               It.Is<DasEnglishFraction>(fraction => IsSameAsFractionCalculation(fraction, _fractionCalculations[3])),
               _employerReference), Times.Once);
        }

        [Test]
        public async Task ThenIShouldUpdateFractionsWithValidCalculatedDates()
        {
            //Assign
            _fractionCalculations[2].CalculatedAt = "this is not a date";

            _englishFractionRepository.Setup(
                x => x.CreateEmployerFraction(It.IsAny<DasEnglishFraction>(), It.IsAny<string>()))
                .Returns(Task.Run(() => { }));

            _englishFractionRepository.Setup(x => x.GetAllEmployerFractions(_employerReference))
                .ReturnsAsync(_existingFractions);

            _hmrcService.Setup(x => x.GetEnglishFractions(It.IsAny<string>(), _employerReference))
                .ReturnsAsync(new EnglishFractionDeclarations
                {
                    Empref = _employerReference,
                    FractionCalculations = _fractionCalculations
                });

            //Act
            await _handler.Handle(new UpdateEnglishFractionsCommand
            {
                EmployerReference = _employerReference
            });

            //Assert
            _englishFractionRepository.Verify(x => x.CreateEmployerFraction(
                It.Is<DasEnglishFraction>(fraction => IsSameAsFractionCalculation(fraction, _fractionCalculations[2])),
                _employerReference), Times.Never);

            _englishFractionRepository.Verify(x => x.CreateEmployerFraction(
               It.Is<DasEnglishFraction>(fraction => IsSameAsFractionCalculation(fraction, _fractionCalculations[3])),
               _employerReference), Times.Once);
        }

        [Test]
        public async Task ThenIShouldUpdateFractionsWithValidAmountValues()
        {
            //Assign
            _fractionCalculations[2].Fractions[0].Value = "this is not an amount";

            _englishFractionRepository.Setup(
                x => x.CreateEmployerFraction(It.IsAny<DasEnglishFraction>(), It.IsAny<string>()))
                .Returns(Task.Run(() => { }));

            _englishFractionRepository.Setup(x => x.GetAllEmployerFractions(_employerReference))
                .ReturnsAsync(_existingFractions);

            _hmrcService.Setup(x => x.GetEnglishFractions(It.IsAny<string>(), _employerReference))
                .ReturnsAsync(new EnglishFractionDeclarations
                {
                    Empref = _employerReference,
                    FractionCalculations = _fractionCalculations
                });

            //Act
            await _handler.Handle(new UpdateEnglishFractionsCommand
            {
                EmployerReference = _employerReference
            });

            //Assert
            _englishFractionRepository.Verify(x => x.CreateEmployerFraction(
                It.Is<DasEnglishFraction>(fraction => IsSameAsFractionCalculation(fraction, _fractionCalculations[2])),
                _employerReference), Times.Never);

            _englishFractionRepository.Verify(x => x.CreateEmployerFraction(
               It.Is<DasEnglishFraction>(fraction => IsSameAsFractionCalculation(fraction, _fractionCalculations[3])),
               _employerReference), Times.Once);
        }

        private static bool IsSameAsFractionCalculation(DasEnglishFraction fraction, FractionCalculation fractionCalculation)
        {
            var fractiondateString = fraction.DateCalculated.ToShortDateString();
            var fractionAmountString = fraction.Amount.ToString(CultureInfo.InvariantCulture);

            var fractionCalculationAmount = fractionCalculation.Fractions.First().Value;

           return fractiondateString.Equals(fractionCalculation.CalculatedAt) &&
                   fractionAmountString.Equals(fractionCalculationAmount);
        }
    }
}
