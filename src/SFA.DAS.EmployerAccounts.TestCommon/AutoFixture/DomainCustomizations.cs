using AutoFixture;
using AutoFixture.AutoMoq;

namespace SFA.DAS.EmployerAccounts.TestCommon.AutoFixture
{
    public class DomainCustomizations : CompositeCustomization
    {
        public DomainCustomizations() : base(
            new AutoMoqCustomization { ConfigureMembers = true })
        {
        }
    }
}
