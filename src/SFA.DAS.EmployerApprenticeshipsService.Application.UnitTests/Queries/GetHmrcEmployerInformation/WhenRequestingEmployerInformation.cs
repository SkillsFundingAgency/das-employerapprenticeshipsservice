using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetHmrcEmployerInformation
{
    public class WhenRequestingEmployerInformation
    {
        private GetHmrcEmployerInformationHandler _getHmrcEmployerInformationHandler;

        [SetUp]
        public void Arrange()
        {
            _getHmrcEmployerInformationHandler = new GetHmrcEmployerInformationHandler();
        }
    }
}
