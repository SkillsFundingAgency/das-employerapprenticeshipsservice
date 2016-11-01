//using System;
//using System.Collections.Generic;
//using System.Linq;
//using NUnit.Framework;
//using SFA.DAS.EAS.LevyAggregationProvider.Worker.Providers;
//using SFA.DAS.EAS.TestCommon.ObjectMothers;

//namespace SFA.DAS.EAS.LevyAggregationProvider.Worker.UnitTests.Providers.LevyAggregationTests
//{
//    public class WhenIAggregateLevyData
//    {
//        private LevyAggregator _levyAggregator;
//        private List<LevyDeclarationSourceDataObjectMother.Emprefs> _expectedEmpref;
//        private List<LevyDeclarationSourceDataObjectMother.Emprefs> _expectedEmprefs;

//        [SetUp]
//        public void Arrange()
//        {
//            var empref1 = new LevyDeclarationSourceDataObjectMother.Emprefs {Empref = "123/ABC123" };
//            var empref2 = new LevyDeclarationSourceDataObjectMother.Emprefs {Empref = "456/ABC456" };
//            _expectedEmpref = new List<LevyDeclarationSourceDataObjectMother.Emprefs> { empref1};
//            _expectedEmprefs = new List<LevyDeclarationSourceDataObjectMother.Emprefs> { empref1, empref2 };
//            _levyAggregator = new LevyAggregator();
//        }

//        [Test]
//        public void ThenNullIsRetrunedIfThereIsNoDataToAggregate()
//        {
//            //Act
//            var actual = _levyAggregator.BuildAggregate(null);

//            //Assert
//            Assert.IsNull(actual);
//        }
        
//        [Test]
//        public void ThenTheAmountMatchesTheLevyYtdForASingleEntry()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            Assert.AreEqual(actualData.Data[0].LevyDueYtd, actual.Data[0].Amount);

//        }

//        [Test]
//        public void ThenLineItemsAreCorrectlyAddedAndGroupedByDate()
//        {
//            //Arrange
            
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmprefs, numberOfDeclarations: 2, declarationsPerperiodPerPaye: 2);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Act
//            Assert.AreEqual(4,actual.Data.First().Items.Count);
//        }

//        [Test]
//        public void ThenWhenThereAreMultipleSubmissionsForAPeriodOnlyTheLatestIsUsed()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref, numberOfDeclarations:2, declarationsPerperiodPerPaye: 2);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            Assert.AreEqual(actualData.Data.FirstOrDefault(c=>c.LastSubmission==1).LevyDueYtd,actual.Data[1].Amount);
//        }

//        [Test]
//        public void ThenTheEnglishFractionIsUsedToWorkOutTheAmount()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref, numberOfDeclarations: 2, declarationsPerperiodPerPaye: 1, randomEnlgishFraction:true);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var levyDeclarationSourceDataItem = actualData.Data[0];
//            var exepectedBalance = levyDeclarationSourceDataItem.LevyDueYtd * levyDeclarationSourceDataItem.EnglishFraction;
//            Assert.AreEqual(exepectedBalance, actual.Data[1].Amount);
//        }


//        [Test]
//        public void ThenWhenThereAreMultipleSubmissionsForMultipleSchemesInAPeriodOnlyTheLatestIsUsed()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmprefs, numberOfDeclarations: 2, declarationsPerperiodPerPaye: 2);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
//            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction)
//                                + (someValue[2].LevyDueYtd * someValue[2].EnglishFraction);
//            var expectedAmount2 = ((someValue[1].LevyDueYtd * someValue[1].EnglishFraction) - (someValue[0].LevyDueYtd * someValue[0].EnglishFraction))
//                                + ((someValue[3].LevyDueYtd * someValue[3].EnglishFraction) - (someValue[2].LevyDueYtd * someValue[2].EnglishFraction));
//            Assert.AreEqual(expectedAmount1, actual.Data[1].Amount);
//            Assert.AreEqual(expectedAmount2, actual.Data[0].Amount);
//        }

//        [Test]
//        public void ThenTheAmountForTheLineIsTheDifferenceFromThePreviousLevyDeclaredYtd()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref, numberOfDeclarations: 2);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var levyDueYtd2 = actualData.Data[0].LevyDueYtd * actualData.Data[0].EnglishFraction;
//            var levyDueYtd1 = (actualData.Data[1].LevyDueYtd - actualData.Data[0].LevyDueYtd) * actualData.Data[1].EnglishFraction;
//            Assert.AreEqual(levyDueYtd1, actual.Data[0].Amount);
//            Assert.AreEqual(levyDueYtd2, actual.Data[1].Amount);
//        }

//        [Test]
//        public void ThenTheBalanceIsTheResultOfTheLatestSubmittedItemsForThatPeriod()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref, numberOfDeclarations: 2, declarationsPerperiodPerPaye:2, randomEnlgishFraction:true);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
//            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction);
//            var expectedAmount2 = ((someValue[1].LevyDueYtd*someValue[1].EnglishFraction) -
//                                   (someValue[0].LevyDueYtd*someValue[0].EnglishFraction));
//            Assert.AreEqual(expectedAmount1, actual.Data[1].Amount);
//            Assert.AreEqual(expectedAmount2, actual.Data[0].Amount);
//            Assert.AreEqual(expectedAmount1+expectedAmount2,actual.Data[0].Balance);
//        }

//        [Test]
//        public void ThenTheBalanceIsTheResultOfTheLatestSubmittedItemsForAllSchemesForThatPeriod()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmprefs, numberOfDeclarations: 2, declarationsPerperiodPerPaye: 2, randomEnlgishFraction: true);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
//            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction)
//                                + (someValue[2].LevyDueYtd * someValue[2].EnglishFraction);
//            var expectedAmount2 = ((someValue[1].LevyDueYtd * someValue[1].EnglishFraction) 
//                                    - (someValue[0].LevyDueYtd * someValue[0].EnglishFraction))
//                                + ((someValue[3].LevyDueYtd * someValue[3].EnglishFraction) 
//                                    - (someValue[2].LevyDueYtd * someValue[2].EnglishFraction));
//           Assert.AreEqual(expectedAmount1 + expectedAmount2, actual.Data[0].Balance);
//        }

//        [Test]
//        public void ThenThePreviousAmountIsUsedToCalculateTheLevyAmountCorrectly()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref, numberOfDeclarations: 3,randomEnlgishFraction:true);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
//            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction);
//            var expectedAmount2 = ((someValue[1].LevyDueYtd * someValue[1].EnglishFraction) -
//                                   (someValue[0].LevyDueYtd * someValue[0].EnglishFraction));
//            var expectedAmount3 = ((someValue[2].LevyDueYtd * someValue[2].EnglishFraction) -
//                                   (someValue[1].LevyDueYtd * someValue[1].EnglishFraction));
//            Assert.AreEqual(expectedAmount1, actual.Data[2].Amount);
//            Assert.AreEqual(expectedAmount2, actual.Data[1].Amount);
//            Assert.AreEqual(expectedAmount3, actual.Data[0].Amount);
//            Assert.AreEqual(expectedAmount1 + expectedAmount2 + expectedAmount3, actual.Data[0].Balance);
//        }

//        [Test]
//        public void ThenIfThereIsATopUpThenItIsAddedToTheAggregationAsASeperateLine()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref, numberOfDeclarations:2,addTopUp:true);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            Assert.AreEqual(4,actual.Data.Sum(c=>c.Items.Count));
//        }

//        [Test]
//        public void ThenTheTotalIncludesTheTopUpValue()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref, numberOfDeclarations: 1, addTopUp: true);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var expectedTotal = actualData.Data[0].LevyDueYtd + actualData.Data[0].TopUp;
//            Assert.AreEqual(expectedTotal,actual.Data[0].Balance);
//        }

//        [Test]
//        public void ThenTheTotalIncludesTheTopUpValueAndTheEnglishPercentage()
//        {
//            //Arrange
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(_expectedEmpref, numberOfDeclarations: 1, addTopUp: true,randomEnlgishFraction:true);

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var expectedTotal = Math.Round(actualData.Data[0].LevyDueYtd * actualData.Data[0].EnglishFraction,2) + Math.Round(actualData.Data[0].TopUp * actualData.Data[0].EnglishFraction,2);
//            Assert.AreEqual(Math.Round(expectedTotal,2), actual.Data[0].Balance);
//        }

//        [Test]
//        public void ThenIfTheAccountIsOpenedWhenThereAreMultipleSubmissionsAlreadyMadeTheyAreAllContainedInOneLineItem()
//        {
//            //Arrange
//            var emprefs = new List<LevyDeclarationSourceDataObjectMother.Emprefs> { new LevyDeclarationSourceDataObjectMother.Emprefs
//            {
//                AddedDate = new DateTime(2017,01,01),
//                Empref = "123/ABC123",
//                RemovedDate = null
//            } };
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(emprefs, numberOfDeclarations: 3, addTopUp: true, randomEnlgishFraction: true,submissionStartDate:new DateTime(2016,01,01));

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            Assert.AreEqual(1,actual.Data.Count);
//        }


//        [Test]
//        public void ThenIfTheAccountIsOpenedWhenThereAreMultipleSubmissionsAlreadyMadeTheyAreAllContainedInOneLineItemAndNewOnesAreSeperate()
//        {
//            //Arrange
//            var emprefs = new List<LevyDeclarationSourceDataObjectMother.Emprefs> { new LevyDeclarationSourceDataObjectMother.Emprefs
//            {
//                AddedDate = new DateTime(2017,01,01),
//                Empref = "123/ABC123",
//                RemovedDate = null
//            } };
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(emprefs, numberOfDeclarations: 3, addTopUp: true, randomEnlgishFraction: true, submissionStartDate: new DateTime(2016, 10, 01));

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            Assert.AreEqual(2, actual.Data.Count);
//        }

//        [Test]
//        public void ThenIfTheAccountIsOpenedWhenThereAreMultipleSubmissionsAlreadyMadeTheyAreAllContainedInOneLineItemforMultipleEmprefs()
//        {
//            //Arrange
//            var emprefs = new List<LevyDeclarationSourceDataObjectMother.Emprefs> { new LevyDeclarationSourceDataObjectMother.Emprefs
//            {
//                AddedDate = new DateTime(2017,01,01),
//                Empref = "123/ABC123",
//                RemovedDate = null
//            },
//            new LevyDeclarationSourceDataObjectMother.Emprefs
//            {
//                AddedDate = new DateTime(2017,01,01),
//                Empref = "456/ABC456",
//                RemovedDate = null
//            }};
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(emprefs, numberOfDeclarations: 3, addTopUp: true, randomEnlgishFraction: true, submissionStartDate: new DateTime(2016, 01, 01));

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            Assert.AreEqual(1, actual.Data.Count);
//        }


//        [Test]
//        public void ThenTheBalanceIsTheResultOfTheLatestSubmittedItemsForThatPeriodWhenContainedOnSingleLine()
//        {
//            //Arrange
//            var emprefs = new List<LevyDeclarationSourceDataObjectMother.Emprefs> { new LevyDeclarationSourceDataObjectMother.Emprefs
//            {
//                AddedDate = new DateTime(2017,01,01),
//                Empref = "123/ABC123",
//                RemovedDate = null
//            } };
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(emprefs, numberOfDeclarations: 2, addTopUp: false, randomEnlgishFraction: true, submissionStartDate: new DateTime(2016, 8, 01));

//            //Act
//            var actual = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
//            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction);
//            var expectedAmount2 = ((someValue[1].LevyDueYtd * someValue[1].EnglishFraction) -
//                                   (someValue[0].LevyDueYtd * someValue[0].EnglishFraction));
//            Assert.AreEqual(expectedAmount1 + expectedAmount2, actual.Data[0].Amount);
//            Assert.AreEqual(expectedAmount1 + expectedAmount2, actual.Data[0].Balance);
//        }

//        [Test]
//        public void ThenTheTotalIsTakenFromThePreviousAccountWhenTheSchemeIsAddedToANewAccount()
//        {
//            //Arrange
//            var emprefs = new List<LevyDeclarationSourceDataObjectMother.Emprefs> { new LevyDeclarationSourceDataObjectMother.Emprefs
//            {
//                AddedDate = new DateTime(2017,01,01),
//                Empref = "123/ABC123",
//                RemovedDate = null,
//                DeclarationsForScheme = 1
//            }};
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(emprefs, numberOfDeclarations: 2, addTopUp: false, randomEnlgishFraction: true,multipleAccountIds:true);

//            //Act
//            var actualAccount1 = _levyAggregator.BuildAggregate(actualData);
//            actualData.AccountId = actualData.AccountId + 1;
//            var actualAccount2 = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            Assert.AreEqual(1, actualAccount1.Data.Count);
//            Assert.AreEqual(1, actualAccount2.Data.Count);
//            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
//            var expectedAmount1 = (someValue[0].LevyDueYtd * someValue[0].EnglishFraction);
//            var expectedAmount2 = ((someValue[1].LevyDueYtd * someValue[1].EnglishFraction) -
//                                   (someValue[0].LevyDueYtd * someValue[0].EnglishFraction));
//            Assert.AreEqual(expectedAmount1, actualAccount1.Data[0].Amount);
//            Assert.AreEqual(expectedAmount2, actualAccount2.Data[0].Amount);
//        }

//        [Test]
//        public void ThenTheTotalIsTakenFromThePreviousAccountWhenTheSchemeIsAddedToANewAccountWhenAddingYourSchemeAfterDeclarationsHaveBeenMade()
//        {
//            //Arrange
//            var emprefs = new List<LevyDeclarationSourceDataObjectMother.Emprefs> { new LevyDeclarationSourceDataObjectMother.Emprefs
//            {
//                AddedDate = new DateTime(2017,01,01),
//                Empref = "123/ABC123",
//                RemovedDate = null,
//                DeclarationsForScheme = 2
//            }};
//            var actualData = LevyDeclarationSourceDataObjectMother.Create(emprefs, numberOfDeclarations: 4, addTopUp: false, submissionStartDate: new DateTime(2016, 8, 01),multipleAccountIds:true);

//            //Act
//            var actualAccount1 = _levyAggregator.BuildAggregate(actualData);
//            actualData.AccountId = actualData.AccountId + 1;
//            var actualAccount2 = _levyAggregator.BuildAggregate(actualData);

//            //Assert
//            Assert.AreEqual(1, actualAccount1.Data.Count);
//            Assert.AreEqual(1, actualAccount2.Data.Count);
//            var someValue = actualData.Data.Where(x => x.LastSubmission == 1).ToList();
//            var expectedAmount1 = (someValue[0].LevyDueYtd) + (someValue[1].LevyDueYtd - someValue[0].LevyDueYtd);
//            var expectedAmount2 = ((someValue[2].LevyDueYtd) - (someValue[1].LevyDueYtd)) + ((someValue[3].LevyDueYtd )- (someValue[2].LevyDueYtd)); 
//            Assert.AreEqual(expectedAmount1, actualAccount1.Data[0].Amount);
//            Assert.AreEqual(expectedAmount2, actualAccount2.Data[0].Amount);

//        }

//        [Test]
//        public void ThenTheValuesAreRoundedToTheNearestPenny()
//        {
//            //Arrange
//            var sourceData = LevyDeclarationSourceDataObjectMother.CreateStatic();

//            //Act
//            var actualData = _levyAggregator.BuildAggregate(sourceData);

//            //Assert
//            var actualLineItem = actualData.Data.FirstOrDefault();
//            Assert.IsNotNull(actualLineItem);
//            Assert.AreEqual(1936.74m, actualLineItem.Amount);
//        }

//        [Test]
//        public void ThenTheTotalsAreRoundedToTheNearestPenny()
//        {
//            //Arrange
//            var sourceData = LevyDeclarationSourceDataObjectMother.CreateStatic(2);

//            //Act
//            var actualData = _levyAggregator.BuildAggregate(sourceData);

//            //Assert
//            Assert.AreEqual(3873.48m, actualData.Data.Last().Balance);
//        }
        
//    }
//}
