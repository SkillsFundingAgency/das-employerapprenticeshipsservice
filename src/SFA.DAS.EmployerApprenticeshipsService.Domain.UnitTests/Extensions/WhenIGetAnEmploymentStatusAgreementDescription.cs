using NUnit.Framework;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

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
            Assert.AreEqual("Superseded", EmployerAgreementStatus.Superseded.GetDescription());
        }
    }
}
