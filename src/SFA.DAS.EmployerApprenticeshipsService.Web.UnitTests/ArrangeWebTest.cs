using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Mappings;

namespace SFA.DAS.EAS.Web.UnitTests
{
    [SetUpFixture]
    public class ArrangeWebTest
    {
        [OneTimeSetUp]
        public void Arrange()
        {
            Mapper.Initialize(c =>
            {
                c.AddProfile<ActivityMaps>();
                c.AddProfile<TransferConnectionInvitationMaps>();
            });
        }
    }
}