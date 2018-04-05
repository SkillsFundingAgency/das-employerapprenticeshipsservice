using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.SendNotification;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.TimeProvider;
using Entity = SFA.DAS.Audit.Types.Entity;

namespace SFA.DAS.EAS.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandHandler : AsyncRequestHandler<CreateInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly EmployerApprenticeshipsServiceConfiguration _employerApprenticeshipsServiceConfiguration;
        private readonly IValidator<CreateInvitationCommand> _validator;
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;

        public CreateInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator, 
            EmployerApprenticeshipsServiceConfiguration employerApprenticeshipsServiceConfiguration, IValidator<CreateInvitationCommand> validator,
            IUserRepository userRepository, IMessagePublisher messagePublisher)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
            _validator = validator;
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(CreateInvitationCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            if (validationResult.IsUnauthorized)
                throw new UnauthorizedAccessException();

            var caller = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            ////Verify the email is not used by an existing invitation for the account
            var existingInvitation = await _invitationRepository.Get(caller.AccountId, message.EmailOfPersonBeingInvited);

            if (existingInvitation != null && existingInvitation.Status != InvitationStatus.Deleted && existingInvitation.Status != InvitationStatus.Accepted)
                throw new InvalidRequestException(new Dictionary<string, string> { { "ExistingMember", $"{message.EmailOfPersonBeingInvited} is already invited" } });

            var expiryDate = DateTimeProvider.Current.UtcNow.Date.AddDays(8);

            var invitationId = 0L;

            if (existingInvitation == null)
            {
                invitationId = await _invitationRepository.Create(new Invitation
                {
                    AccountId = caller.AccountId,
                    Email = message.EmailOfPersonBeingInvited,
                    Name = message.NameOfPersonBeingInvited,
                    RoleId = message.RoleIdOfPersonBeingInvited,
                    Status = InvitationStatus.Pending,
                    ExpiryDate = expiryDate
                });
            }
            else
            {
                existingInvitation.Name = message.NameOfPersonBeingInvited;
                existingInvitation.RoleId = message.RoleIdOfPersonBeingInvited;
                existingInvitation.Status = InvitationStatus.Pending;
                existingInvitation.ExpiryDate = expiryDate;

                await _invitationRepository.Resend(existingInvitation);

                invitationId = existingInvitation.Id;
            }

            var existingUser = await _userRepository.GetByEmailAddress(message.EmailOfPersonBeingInvited);

            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "CREATED",
                    Description = $"Member {message.EmailOfPersonBeingInvited} added to account {caller.AccountId} as {message.RoleIdOfPersonBeingInvited}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        
                        PropertyUpdate.FromString("AccountId",caller.AccountId.ToString()),
                        PropertyUpdate.FromString("Email",message.EmailOfPersonBeingInvited),
                        PropertyUpdate.FromString("Name",message.NameOfPersonBeingInvited),
                        PropertyUpdate.FromString("RoleId",message.RoleIdOfPersonBeingInvited.ToString()),
                        PropertyUpdate.FromString("Status",InvitationStatus.Pending.ToString()),
                        PropertyUpdate.FromDateTime("ExpiryDate",expiryDate)
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = caller.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Invitation", Id = invitationId.ToString() }
                }
            });

            await _mediator.SendAsync(new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = message.EmailOfPersonBeingInvited,
                    TemplateId = existingUser?.UserRef != null ? "InvitationExistingUser" : "InvitationNewUser",
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "x",
                    SystemId = "x",
                    Tokens = new Dictionary<string, string> {
                        { "account_name", caller.AccountName },
                        { "first_name", message.NameOfPersonBeingInvited },
                        { "inviter_name", $"{caller.FirstName} {caller.LastName}"},
                        { "base_url", _employerApprenticeshipsServiceConfiguration.DashboardUrl },
                        { "expiry_date", expiryDate.ToString("dd MMM yyy")}
                    }
                }
            });

            await PublishAccountCreatedMessage(caller.AccountId, caller.FullName(), message.NameOfPersonBeingInvited, caller.UserRef);
        }

        private async Task PublishAccountCreatedMessage(long accountId, string signedByName, string personInvited, string userRef)
        {
            await _messagePublisher.PublishAsync(new UserInvitedMessage(personInvited, accountId, signedByName, userRef));
        }
    }
}