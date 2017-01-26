using NUnit.Framework;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.EmployerAgreement;
using SFA.DAS.EAS.Domain.Extensions;

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
