using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.Provider.Events.Api.Types;
using StructureMap.TypeRules;
using Payment = SFA.DAS.Provider.Events.Api.Types.Payment;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Mapping.PaymentMappingTests
{
    class WhenIMapAPaymentToPaymentDetails
    {
        private IMapper _mapper;

        [SetUp]
        public void Arrange()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("SFA.DAS.EAS"));

            var mappingProfiles = new List<Profile>();

            foreach (var assembly in assemblies)
            {
                var profiles = Assembly.Load(assembly.FullName).GetTypes()
                    .Where(t => typeof(Profile).IsAssignableFrom(t))
                    .Where(t => t.IsConcrete() && t.HasConstructors())
                    .Select(t => (Profile)Activator.CreateInstance(t));

                mappingProfiles.AddRange(profiles);
            }

            var config = new MapperConfiguration(cfg =>
            {
                mappingProfiles.ForEach(cfg.AddProfile);
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        public void ThenAllDataShouldBeMappedCorrectly()
        {
            //Arrange
            var payment = new Payment
            {
                Id = "1",
                Ukprn = 321231,
                Amount = 120.67m,
                ApprenticeshipId = 123,
                DeliveryPeriod = new CalendarPeriod { Month = 4, Year = 1956},
                CollectionPeriod = new NamedCalendarPeriod { Id="564", Month = 6, Year = 2018},
                TransactionType = TransactionType.Learning,
                ApprenticeshipVersion = "1.1",
                EmployerAccountVersion = "1.2",
                ContractType = ContractType.ContractWithEmployer,
                EmployerAccountId = "7897",
                FrameworkCode = 12,
                EvidenceSubmittedOn = DateTime.Now,
                FundingSource = FundingSource.CoInvestedSfa,
                PathwayCode = 4,
                ProgrammeType = 3,
                StandardCode = 78,
                Uln = 5555
            };

            //Act
            var result = _mapper.Map<PaymentDetails>(payment);

            //Assert
            Assert.AreEqual(payment.Id, result.Id);
            Assert.AreEqual(payment.Ukprn, result.Ukprn);
            Assert.AreEqual(payment.Amount, result.Amount);
            Assert.AreEqual(payment.ApprenticeshipId, result.ApprenticeshipId);
            Assert.AreEqual(payment.DeliveryPeriod.Month, result.DeliveryPeriodMonth);
            Assert.AreEqual(payment.DeliveryPeriod.Year, result.DeliveryPeriodYear);
            Assert.AreEqual(payment.CollectionPeriod.Id, result.CollectionPeriodId);
            Assert.AreEqual(payment.CollectionPeriod.Month, result.CollectionPeriodMonth);
            Assert.AreEqual(payment.CollectionPeriod.Year, result.CollectionPeriodYear);
            Assert.AreEqual(payment.TransactionType, result.TransactionType);
            Assert.AreEqual(payment.ApprenticeshipVersion, result.ApprenticeshipVersion);
            Assert.AreEqual(payment.EmployerAccountVersion, result.EmployerAccountVersion);
            Assert.AreEqual(payment.EmployerAccountId, result.EmployerAccountId.ToString());
            Assert.AreEqual(payment.FrameworkCode, result.FrameworkCode);
            Assert.AreEqual(payment.EvidenceSubmittedOn, result.EvidenceSubmittedOn);
            Assert.AreEqual(payment.FundingSource, result.FundingSource);
            Assert.AreEqual(payment.PathwayCode, result.PathwayCode);
            Assert.AreEqual(payment.ProgrammeType, result.ProgrammeType);
            Assert.AreEqual(payment.StandardCode, result.StandardCode);
            Assert.AreEqual(payment.Uln, result.Uln);
        }
    }
}
