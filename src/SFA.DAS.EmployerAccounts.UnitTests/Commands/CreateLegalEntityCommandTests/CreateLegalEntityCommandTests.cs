using System;
using MediatR;
using Moq;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public abstract class CreateLegalEntityCommandTests
    {
        protected Mock<IAccountRepository> AccountRepository;
        protected Mock<IMembershipRepository> MembershipRepository;
        protected Mock<IMediator> Mediator;
        protected Mock<IGenericEventFactory> GenericEventFactory;
        protected CreateLegalEntityCommandHandler CommandHandler;
        protected CreateLegalEntityCommand Command;
        protected MembershipView Owner;
        protected EmployerAgreementView AgreementView;
        protected Mock<ILegalEntityEventFactory> LegalEntityEventFactory;
        protected Mock<IEncodingService> EncodingService;
        protected Mock<IEmployerAgreementRepository> EmployerAgreementRepository;
        protected Mock<IValidator<CreateLegalEntityCommand>> Validator;
        protected Mock<IEventPublisher> EventPublisher;
        protected const long AccountId = 123435;

        public virtual void Arrange()
        {
            Command = new CreateLegalEntityCommand
            {
                HashedAccountId = "ABC123",
                ExternalUserId = Guid.NewGuid().ToString(),
                Name = "Org Ltd",
                Code = "3476782638",
                Source = OrganisationType.CompaniesHouse,
                Address = "123 test street"
            };

            AccountRepository = new Mock<IAccountRepository>();
            AccountRepository.Setup(x => x.CreateLegalEntityWithAgreement(It.IsAny<CreateLegalEntityWithAgreementParams>())).ReturnsAsync(new EmployerAgreementView());

            MembershipRepository = new Mock<IMembershipRepository>();
            MembershipRepository.Setup(x => x.GetCaller(Command.HashedAccountId, Command.ExternalUserId)).ReturnsAsync(new MembershipView { UserRef = Guid.Parse(Command.ExternalUserId), AccountId = AccountId });

            Mediator = new Mock<IMediator>();
            GenericEventFactory = new Mock<IGenericEventFactory>();
            LegalEntityEventFactory = new Mock<ILegalEntityEventFactory>();
            EncodingService = new Mock<IEncodingService>();
            EmployerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            EventPublisher = new Mock<IEventPublisher>();

            Validator = new Mock<IValidator<CreateLegalEntityCommand>>();
            Validator.Setup(x => x.ValidateAsync(It.IsAny<CreateLegalEntityCommand>())).ReturnsAsync(new ValidationResult() { IsUnauthorized = false });

            CommandHandler = new CreateLegalEntityCommandHandler(
                AccountRepository.Object,
                MembershipRepository.Object,
                Mediator.Object,
                GenericEventFactory.Object,
                LegalEntityEventFactory.Object,
                EventPublisher.Object,
                EncodingService.Object,
                EmployerAgreementRepository.Object,
                Validator.Object
            );
        }
    }
}
