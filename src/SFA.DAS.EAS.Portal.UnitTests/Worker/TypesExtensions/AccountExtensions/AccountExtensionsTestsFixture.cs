using System;
using AutoFixture;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.TypesExtensions.AccountExtensions
{
    public class AccountExtensionsTestsFixture<T>
    {
        public Account Account { get; set; }
        public Account OriginalAccount { get; set; }
        public Fixture Fixture { get; set; }
        public Action<T> OnAdd { get; set; }
        public Action<T> OnGet { get; set; }

        public AccountExtensionsTestsFixture()
        {
            Fixture = new Fixture();
            Account = Fixture.Create<Account>();
            Account.Deleted = null;
        }
    }
}