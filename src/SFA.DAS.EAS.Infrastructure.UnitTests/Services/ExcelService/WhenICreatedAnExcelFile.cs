using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.ExcelService
{
    class WhenICreatedAnExcelFile
    {
        private Infrastructure.Services.ExcelService _service;

        [SetUp]
        public void Arrange()
        {
            _service = new Infrastructure.Services.ExcelService();
        }

        [Test]
        public void ThenIShouldBeAbleToConvertItBackToData()
        {
            //Arrange
            var expectedData = new[]
            {
                new[] {"A", "B", "C"}, 
                new[] {"D", "E", "F"}
            };
            var worksheetName = "TestData";

            //Act
            var excelData = _service.CreateExcelFile(new Dictionary<string, string[][]>
            {
                {worksheetName, expectedData}
            });

            var actualData = _service.ReadExcelFile(excelData);

            //Assert
            Assert.IsTrue(actualData.ContainsKey(worksheetName), "Worksheet was not found");
            Assert.AreEqual(expectedData, actualData[worksheetName], "Excel data did not get written or read correctly");
        }

        [Test]
        public void ThenAnExceptionShouldBeThrowIfNoDataIsBeingWritten()
        {
            //Act + Assert
            Assert.Throws<ArgumentException>(() => _service.CreateExcelFile(new Dictionary<string, string[][]>()));
        }
    }
}
