using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Extensions;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.UnitTests.Extensions
{
    class WhenIGetAnEmploymentStatusAgreementDescription
    {
        [Test]
        public void ThenIShouldGetTheCorrectDescription()
        {
            Assert.AreEqual("Not signed", EmployerAgreementStatus.Pending.GetDescription());
            Assert.AreEqual("Signed", EmployerAgreementStatus.Signed.GetDescription());
            Assert.AreEqual("Expired", EmployerAgreementStatus.Expired.GetDescription());
            Assert.AreEqual("Superceded", EmployerAgreementStatus.Superceded.GetDescription());
        }
    }
}
