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
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Client.Database.Models;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.EventHandlers
{
    public class EventHandlerTestsFixture<TEvent, TEventHandler>
        where TEventHandler : IEventHandler<TEvent>
    {
        public TEvent Message { get; set; }
        public TEvent OriginalMessage { get; set; }
        public IEventHandler<TEvent> Handler { get; set; }
//        public string MessageId { get; set; }
        //public Mock<IMessageContextInitialisation> MessageContextInitialisation { get; set; }
        //public Mock<IMessageHandlerContext> MessageHandlerContext { get; set; }
        public Mock<IAccountDocumentService> AccountDocumentService { get; set; }
        public AccountDocument AccountDocument { get; set; }
        public AccountDocument OriginalAccountDocument { get; set; }
        public Mock<ILogger<TEventHandler>> Logger { get; set; }
        public Fixture Fixture { get; set; }
        
        public EventHandlerTestsFixture(bool constructHandler = true)
        {
            Fixture = new Fixture();
            //todo: doesn't belong in common
            Fixture.Customize<Cohort>(co => co.With(x => x.IsApproved, false));

            Message = Fixture.Create<TEvent>();

//            MessageContextInitialisation = new Mock<IMessageContextInitialisation>();
            
//            MessageId = Fixture.Create<string>();
//            MessageHandlerContext = new Mock<IMessageHandlerContext>();
//            MessageHandlerContext.Setup(c => c.MessageId).Returns(MessageId);

//            var messageHeaders = new Mock<IReadOnlyDictionary<string, string>>();
//            messageHeaders.SetupGet(c => c["NServiceBus.TimeSent"]).Returns(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss:ffffff Z", CultureInfo.InvariantCulture));
//            MessageHandlerContext.Setup(c => c.MessageHeaders).Returns(messageHeaders.Object);

            AccountDocumentService = new Mock<IAccountDocumentService>();
            
            Logger = new Mock<ILogger<TEventHandler>>();
            
            if (constructHandler)
                Handler = ConstructHandler();
        }

        public EventHandlerTestsFixture<TEvent, TEventHandler> ArrangeEmptyAccountDocument(long accountId)
        {
            AccountDocument = JsonConvert.DeserializeObject<AccountDocument>($"{{\"Account\": {{\"Id\": {accountId} }}}}");

            AccountDocumentService.Setup(s => s.Get(accountId, It.IsAny<CancellationToken>())).ReturnsAsync(AccountDocument);
            
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

//        public void VerifyMessageContextIsInitialised()
//        {
//            MessageContextInitialisation.Verify(mc => mc.Initialise(MessageHandlerContext.Object), Times.Once);
//        }
        
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