using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    [TestFixture]
    public class AgreementTemplateExtensionsTests
    {
        [TestCase(1, "")]
        [TestCase(2, "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding).")]
        [TestCase(3, "This is a new agreement.")]
        [TestCase(4, "This is a new agreement.")]
        [TestCase(5, "This is a new agreement.")]
        public void InsetTextSetCorrectlyForNoPreviouslySignedAgreements(int agreementVersion, string expectedResult)
        {
            var model = new AgreementTemplateViewModel
            {
                VersionNumber = agreementVersion
            };

            Assert.AreEqual(expectedResult, model.InsetText(new List<OrganisationAgreementViewModel>()));
        }

        [TestCase(1, "")]
        [TestCase(2, "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding).")]
        [TestCase(3, "This is a new agreement.")]
        [TestCase(4, "This is a new agreement.")]
        [TestCase(5, "This is a new agreement.")]
        public void InsetTextSetCorrectlyForPreviouslySignedv1Agreements(int agreementVersion, string expectedResult)
        {
            var model = new AgreementTemplateViewModel
            {
                VersionNumber = agreementVersion
            };

            var agreements = new List<OrganisationAgreementViewModel>
            {
                new OrganisationAgreementViewModel
                {
                    SignedDate = DateTime.Today,
                    Template = new AgreementTemplateViewModel
                    {
                        VersionNumber = 1
                    }
                }
            };

            Assert.AreEqual(expectedResult, model.InsetText(agreements));
        }

        [TestCase(1, "")]
        [TestCase(2, "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding).")]
        [TestCase(3, "This is a new agreement.")]
        [TestCase(4, "This is a new agreement.")]
        [TestCase(5, "This is a new agreement.")]
        public void InsetTextSetCorrectlyForPreviouslySignedv2Agreements(int agreementVersion, string expectedResult)
        {
            var model = new AgreementTemplateViewModel
            {
                VersionNumber = agreementVersion
            };

            var agreements = new List<OrganisationAgreementViewModel>
            {
                new OrganisationAgreementViewModel
                {
                    SignedDate = DateTime.Today,
                    Template = new AgreementTemplateViewModel
                    {
                        VersionNumber = 2
                    }
                }
            };

            Assert.AreEqual(expectedResult, model.InsetText(agreements));
        }

        [TestCase(1, "")]
        [TestCase(2, "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding).")]
        [TestCase(3, "This is a new agreement.")]
        [TestCase(4, "This is a variation to the agreement we published 9 January 2020. You only need to accept it if you want to access incentive payments for hiring a new apprentice.")]
        [TestCase(5, "This is a variation to the agreement we published 9 January 2020.")]
        public void InsetTextSetCorrectlyForPreviouslySignedv3Agreements(int agreementVersion, string expectedResult)
        {
            var model = new AgreementTemplateViewModel
            {
                VersionNumber = agreementVersion
            };

            var agreements = new List<OrganisationAgreementViewModel>
            {
                new OrganisationAgreementViewModel
                {
                    SignedDate = DateTime.Today,
                    Template = new AgreementTemplateViewModel
                    {
                        VersionNumber = 3
                    }
                }
            };

            Assert.AreEqual(expectedResult, model.InsetText(agreements));
        }

        [TestCase(1, "")]
        [TestCase(2, "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding).")]
        [TestCase(3, "This is a new agreement.")]
        [TestCase(4, "This is a variation to the agreement we published 9 January 2020. You only need to accept it if you want to access incentive payments for hiring a new apprentice.")]
        [TestCase(5, "This is a variation to the agreement we published 9 January 2020. You only need to accept it if you want to access incentive payments for hiring a new apprentice.")]
        public void InsetTextSetCorrectlyForPreviouslySignedv4Agreements(int agreementVersion, string expectedResult)
        {
            var model = new AgreementTemplateViewModel
            {
                VersionNumber = agreementVersion
            };

            var agreements = new List<OrganisationAgreementViewModel>
            {
                new OrganisationAgreementViewModel
                {
                    SignedDate = DateTime.Today,
                    Template = new AgreementTemplateViewModel
                    {
                        VersionNumber = 4
                    }
                }
            };

            Assert.AreEqual(expectedResult, model.InsetText(agreements));
        }
    }
}
