using System.Web;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests
{
    public class Class1
    {
        private Mock<HttpContextBase> _httpContextBase;

        [SetUp]
        public void Arrange()
        {
            _httpContextBase = new Mock<HttpContextBase>();

        }

        [Test]
        public void TestAssert()
        {
            Assert.IsTrue(true);
        }
    }
}
