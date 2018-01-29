using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.IntegrationTests.EmployerAccountControllerTests
{
    class WhenICreateAnAccount 
    {
        [SetUp]
        public void Arrange()
        {
        }
    }
}
