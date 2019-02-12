using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.SendNotification;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.TimeProvider;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;

namespace SFA.DAS.EmployerAccounts.Commands.ResendInvitation
{
    public class ResendInvitationCommandHandler : AsyncRequestHandler<ResendInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly EmployerAccountsConfiguration _employerApprenticeshipsServiceConfiguration;
        private readonly IUserAccountRepository _userRepository;
        private readonly ResendInvitationCommandValidator _validator;

        public ResendInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator, EmployerAccountsConfiguration employerApprenticeshipsServiceConfiguration, IUserAccountRepository userRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (employerApprenticeshipsServiceConfiguration == null)
                throw new ArgumentNullException(nameof(employerApprenticeshipsServiceConfiguration));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
            _userRepository = userRepository;
            _validator = new ResendInvitationCommandValidator();
        }

        protected override async Task HandleCore(ResendInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (owner == null || owner.Role != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var existing = await _invitationRepository.Get(owner.AccountId, message.Email);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation not found" } });

            if (existing.Status == InvitationStatus.Accepted)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Accepted invitations cannot be resent" } });

            existing.Status = InvitationStatus.Pending;
            var expiryDate = DateTimeProvider.Current.UtcNow.Date.AddDays(8);
            existing.ExpiryDate = expiryDate;
            
            await _invitationRepository.Resend(existing);

            var existingUser = await _userRepository.Get(message.Email);

            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "INVITATION_RESENT",
                    Description = $"Invitation to {message.Email} resent in Account {existing.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "Status",NewValue = existing.Status.ToString()},
                        new PropertyUpdate {PropertyName = "ExpiryDate",NewValue = existing.ExpiryDate.ToString()}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = existing.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Invitation", Id = existing.Id.ToString() }
                }
            });
            
            await _mediator.SendAsync(new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = message.Email,
                    TemplateId = existingUser?.UserRef != null ? "InvitationExistingUser" : "InvitationNewUser",
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "x",
                    SystemId = "x",
                    Tokens = new Dictionary<string, string> {
                        { "account_name", owner.AccountName },
                        { "first_name", message.FirstName },
                        { "inviter_name", $"{owner.FirstName} {owner.LastName}"},
                        { "base_url", _employerApprenticeshipsServiceConfiguration.DashboardUrl },
                        { "expiry_date", expiryDate.ToString("dd MMM yyy")}
                    }
                }
            });
        }
    }
}