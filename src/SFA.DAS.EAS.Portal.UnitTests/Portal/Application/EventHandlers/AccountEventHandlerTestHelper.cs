using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services.AccountDocumentService;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers
{
    public class AccountEventHandlerTestHelper<TEvent, TEventHandler>
        where TEventHandler : IEventHandler<TEvent>
    {
        public TEvent Message { get; set; }
        public TEvent OriginalMessage { get; set; }
        public IEventHandler<TEvent> Handler { get; set; }
        public Mock<IAccountDocumentService> AccountDocumentService { get; set; }
        public AccountDocument AccountDocument { get; set; }
        public AccountDocument OriginalAccountDocument { get; set; }
        public Mock<ILogger<TEventHandler>> Logger { get; set; }
        public Fixture Fixture { get; set; }
        
        public AccountEventHandlerTestHelper(bool constructHandler = true)
        {
            Fixture = new Fixture();
            Fixture.Customize<Account>(a => a
                .Without(ac => ac.VacanciesRetrieved)
                .Without(ac => ac.Vacancies));

            Message = Fixture.Create<TEvent>();

            AccountDocumentService = new Mock<IAccountDocumentService>();
            
            Logger = new Mock<ILogger<TEventHandler>>();
            
            if (constructHandler)
                Handler = ConstructHandler();
        }

        public AccountEventHandlerTestHelper<TEvent, TEventHandler> ArrangeAccountDoesNotExist(long accountId)
        {
            AccountDocument = new AccountDocument(accountId);
            
            AccountDocumentService.Setup(s => s.GetOrCreate(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(AccountDocument);

            return this;
        }

        public AccountEventHandlerTestHelper<TEvent, TEventHandler> ArrangeEmptyAccountDocument(long accountId)
        {
            AccountDocument = JsonConvert.DeserializeObject<AccountDocument>($"{{\"Account\": {{\"Id\": {accountId} }}}}");

            AccountDocumentService.Setup(s => s.GetOrCreate(accountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
            return this;
        }
        
        public Organisation SetUpAccountDocumentWithOrganisation(long accountId, long accountLegalEntityId)
        {
            AccountDocument = Fixture.Create<AccountDocument>();

            AccountDocument.Account.Id = accountId;
            
            AccountDocument.Deleted = null;
            AccountDocument.Account.Deleted = null;
            
            var organisation = AccountDocument.Account.Organisations.RandomElement();
            organisation.AccountLegalEntityId = accountLegalEntityId;

            return organisation;
        }
        
        public Task Handle()
        {
            OriginalMessage = Message.Clone();
            OriginalAccountDocument = AccountDocument.Clone();
            return Handler.Handle(Message);
        }

        private TEventHandler ConstructHandler()
        {
            return (TEventHandler)Activator.CreateInstance(typeof(TEventHandler), AccountDocumentService.Object, Logger.Object);
        }

        public Account GetExpectedAccount(long accountId)
        {
            if (OriginalAccountDocument == null)
            {
                return new Account
                {
                    Id = accountId,
                };
            }
            return OriginalAccountDocument.Account;
        }

        public Organisation GetExpectedOrganisation(Account expectedAccount, long accountLegalEntityId, string accountLegalEntityName)
        {
            if (OriginalAccountDocument != null && OriginalAccountDocument.Account.Organisations.Any())
                return expectedAccount.Organisations.Single(o => o.AccountLegalEntityId == accountLegalEntityId);

            var expectedOrganisation = new Organisation
            {
                AccountLegalEntityId = accountLegalEntityId,
                Name = accountLegalEntityName
            };
            expectedAccount.Organisations.Add(expectedOrganisation);

            return expectedOrganisation;
        }
        
        public bool AccountIsAsExpected(Account expectedAccount, AccountDocument savedAccountDocument)
        {
            if (savedAccountDocument?.Account == null)
                return false;

            var (accountIsAsExpected, differences) = savedAccountDocument.Account.IsEqual(expectedAccount);
            if (!accountIsAsExpected)
            {
                TestContext.WriteLine($"Saved account is not as expected: {differences}");
            }

            return accountIsAsExpected;
        }
    }
}