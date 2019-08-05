using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;

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
        protected Mock<IHashingService> HashingService;
        protected Mock<IAccountLegalEntityPublicHashingService> AccountLegalEntityPublicHashingService;
        protected Mock<IAgreementService> AgreementService;
        protected Mock<IEmployerAgreementRepository> EmployerAgreementRepository;
        protected Mock<IValidator<CreateLegalEntityCommand>> Validator;
        protected Mock<IAuthorizationService> AuthorizationService;
        protected Mock<IEventPublisher> EventPublisher;
        protected const long AccountId = 123435;

        public virtual void Arrange()
        {
            Command = new CreateLegalEntityCommand
            {
                HashedAccountId = "ABC123",
                SignAgreement = true,
                SignedDate = DateTime.Now.AddDays(-10),
                ExternalUserId = Guid.NewGuid().ToString(),
                Name = "Org Ltd",
                Code = "3476782638",
                Source = OrganisationType.CompaniesHouse,
                Address = "123 test street"
            };

            AccountRepository = new Mock<IAccountRepository>();
            AccountRepository.Setup(x => x.CreateLegalEntityWithAgreement(It.IsAny<CreateLegalEntityWithAgreementParams>())).ReturnsAsync(new EmployerAgreementView());

            MembershipRepository = new Mock<IMembershipRepository>();
            MembershipRepository.Setup(x => x.GetCaller(Command.HashedAccountId, Command.ExternalUserId)).ReturnsAsync(new MembershipView { UserRef = Command.ExternalUserId, AccountId = AccountId });

            Mediator = new Mock<IMediator>();
            AuthorizationService = new Mock<IAuthorizationService>();
            GenericEventFactory = new Mock<IGenericEventFactory>();
            LegalEntityEventFactory = new Mock<ILegalEntityEventFactory>();
            HashingService = new Mock<IHashingService>();
            AgreementService = new Mock<IAgreementService>();
            AccountLegalEntityPublicHashingService = new Mock<IAccountLegalEntityPublicHashingService>();
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
                HashingService.Object,
                AccountLegalEntityPublicHashingService.Object,
                AgreementService.Object,
                EmployerAgreementRepository.Object,
                Validator.Object,
                AuthorizationService.Object
            );
        }
    }
}
