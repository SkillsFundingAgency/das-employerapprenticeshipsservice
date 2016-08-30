using System.Linq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.TestCommon.ObjectMothers;
using SFA.DAS.LevyAggregationProvider.Worker.Providers;

namespace SFA.DAS.LevyAggregationProvider.Worker.UnitTests.Providers.LevyAggregationTests
{
    public class WhenIAggregateLevyData
    {
        private LevyAggregator _levyAggregator;

        [SetUp]
        public void Arrange()
        {
            _levyAggregator = new LevyAggregator();
        }

        [Test]
        public void ThenNullIsRetrunedIfThereIsNoDataToAggregate()
        {
            //Act
            var actual = _levyAggregator.BuildAggregate(null);

            //Assert
            Assert.IsNull(actual);
        }
        
        [Test]
        public void ThenTheAmountMatchesTheLevyYtdForASingleEntry()
        {
            //Arrange
            var expectedEmprefs= new []{"123/ABC123"};
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            Assert.AreEqual(actualData.Data[0].LevyDueYtd, actual.Data[0].Amount);

        }

        [Test]
        public void ThenLineItemsAreCorrectlyAddedAndGroupedByDate()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123", "456/ABC456" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 2, declarationsPerperiodPerPaye: 2);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Act
            Assert.AreEqual(4,actual.Data.First().Items.Count);
        }

        [Test]
        public void ThenWhenThereAreMultipleSubmissionsForAPeriodOnlyTheLatestIsUsed()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs,numberOfDeclarations:2, declarationsPerperiodPerPaye: 2);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            Assert.AreEqual(actualData.Data.FirstOrDefault(c=>c.LastSubmission==1).LevyDueYtd,actual.Data[1].Amount);
        }

        [Test]
        public void ThenTheEnglishFractionIsUsedToWorkOutTheAmount()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 2, declarationsPerperiodPerPaye: 1, randomEnlgishFraction:true);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            var levyDeclarationSourceDataItem = actualData.Data[0];
            var exepectedBalance = levyDeclarationSourceDataItem.LevyDueYtd * levyDeclarationSourceDataItem.EnglishFraction;
            Assert.AreEqual(exepectedBalance, actual.Data[1].Amount);
        }


        [Test]
        public void ThenWhenThereAreMultipleSubmissionsForMultipleSchemesInAPeriodOnlyTheLatestIsUsed()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" ,"456/ABC456"};
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 2, declarationsPerperiodPerPaye: 2);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction)
                                + (someValue[2].LevyDueYtd * someValue[2].EnglishFraction);
            var expectedAmount2 = ((someValue[1].LevyDueYtd * someValue[1].EnglishFraction) - (someValue[0].LevyDueYtd * someValue[0].EnglishFraction))
                                + ((someValue[3].LevyDueYtd * someValue[3].EnglishFraction) - (someValue[2].LevyDueYtd * someValue[2].EnglishFraction));
            Assert.AreEqual(expectedAmount1, actual.Data[1].Amount);
            Assert.AreEqual(expectedAmount2, actual.Data[0].Amount);
        }

        [Test]
        public void ThenTheAmountForTheLineIsTheDifferenceFromThePreviousLevyDeclaredYtd()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 2);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            var levyDueYtd2 = actualData.Data[0].LevyDueYtd * actualData.Data[0].EnglishFraction;
            var levyDueYtd1 = (actualData.Data[1].LevyDueYtd - actualData.Data[0].LevyDueYtd) * actualData.Data[1].EnglishFraction;
            Assert.AreEqual(levyDueYtd1, actual.Data[0].Amount);
            Assert.AreEqual(levyDueYtd2, actual.Data[1].Amount);
        }

        [Test]
        public void ThenTheBalanceIsTheResultOfTheLatestSubmittedItemsForThatPeriod()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 2, declarationsPerperiodPerPaye:2, randomEnlgishFraction:true);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction);
            var expectedAmount2 = ((someValue[1].LevyDueYtd*someValue[1].EnglishFraction) -
                                   (someValue[0].LevyDueYtd*someValue[0].EnglishFraction));
            Assert.AreEqual(expectedAmount1, actual.Data[1].Amount);
            Assert.AreEqual(expectedAmount2, actual.Data[0].Amount);
            Assert.AreEqual(expectedAmount1+expectedAmount2,actual.Data[0].Balance);
        }

        [Test]
        public void ThenTheBalanceIsTheResultOfTheLatestSubmittedItemsForAllSchemesForThatPeriod()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123", "456/456VBF" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 2, declarationsPerperiodPerPaye: 2, randomEnlgishFraction: true);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction)
                                + (someValue[2].LevyDueYtd * someValue[2].EnglishFraction);
            var expectedAmount2 = ((someValue[1].LevyDueYtd * someValue[1].EnglishFraction) 
                                    - (someValue[0].LevyDueYtd * someValue[0].EnglishFraction))
                                + ((someValue[3].LevyDueYtd * someValue[3].EnglishFraction) 
                                    - (someValue[2].LevyDueYtd * someValue[2].EnglishFraction));
           Assert.AreEqual(expectedAmount1 + expectedAmount2, actual.Data[0].Balance);
        }

        [Test]
        public void ThenThePreviousAmountIsUsedToCalculateTheLevyAmountCorrectly()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 3,randomEnlgishFraction:true);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction);
            var expectedAmount2 = ((someValue[1].LevyDueYtd * someValue[1].EnglishFraction) -
                                   (someValue[0].LevyDueYtd * someValue[0].EnglishFraction));
            var expectedAmount3 = ((someValue[2].LevyDueYtd * someValue[2].EnglishFraction) -
                                   (someValue[1].LevyDueYtd * someValue[1].EnglishFraction));
            Assert.AreEqual(expectedAmount1, actual.Data[2].Amount);
            Assert.AreEqual(expectedAmount2, actual.Data[1].Amount);
            Assert.AreEqual(expectedAmount3, actual.Data[0].Amount);
            Assert.AreEqual(expectedAmount1 + expectedAmount2 + expectedAmount3, actual.Data[0].Balance);
        }

        [Test]
        public void ThenIfThereIsATopUpThenItIsAddedToTheAggregationAsASeperateLine()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations:2,addTopUp:true);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            Assert.AreEqual(4,actual.Data.Sum(c=>c.Items.Count));
        }

        [Test]
        public void ThenTheTotalIncludesTheTopUpValue()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 1, addTopUp: true);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            var expectedTotal = actualData.Data[0].LevyDueYtd + actualData.Data[0].TopUp;
            Assert.AreEqual(expectedTotal,actual.Data[0].Balance);
        }

        [Test]
        public void ThenTheTotalIncludesTheTopUpValueAndTheEnglishPercentage()
        {
            //Arrange
            var expectedEmprefs = new[] { "123/ABC123" };
            var actualData = LevyDeclarationSourceDataObjectMother.Create(expectedEmprefs, numberOfDeclarations: 1, addTopUp: true,randomEnlgishFraction:true);

            //Act
            var actual = _levyAggregator.BuildAggregate(actualData);

            //Assert
            var expectedTotal = (actualData.Data[0].LevyDueYtd * actualData.Data[0].EnglishFraction) + (actualData.Data[0].TopUp * actualData.Data[0].EnglishFraction);
            Assert.AreEqual(expectedTotal, actual.Data[0].Balance);
        }

        [Test]
        public void ThenIfTheAccountIsOpenedWhenThereAreMultipleSubmissionsAlreadyMadeTheyAreAllContainedInOneLineItem()
        {
            
        }
        

    }
}
