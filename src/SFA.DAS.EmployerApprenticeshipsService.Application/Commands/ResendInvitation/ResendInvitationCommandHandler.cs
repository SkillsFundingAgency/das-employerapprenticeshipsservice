using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.SendNotification;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EAS.Application.Commands.ResendInvitation
{
    public class ResendInvitationCommandHandler : AsyncRequestHandler<ResendInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly EmployerApprenticeshipsServiceConfiguration _employerApprenticeshipsServiceConfiguration;
        private readonly ResendInvitationCommandValidator _validator;

        public ResendInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator, EmployerApprenticeshipsServiceConfiguration employerApprenticeshipsServiceConfiguration)
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
            _validator = new ResendInvitationCommandValidator();
        }

        protected override async Task HandleCore(ResendInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (owner == null || (Role)owner.RoleId != Role.Owner)
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
                    TemplateId = _employerApprenticeshipsServiceConfiguration.EmailTemplates.Single(c=>c.TemplateType.Equals(EmailTemplateType.Invitation)).Key,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = "x",
                    SystemId = "x",
                    Tokens = new Dictionary<string, string> {
                        { "account_name", owner.AccountName },
                        { "base_url", _employerApprenticeshipsServiceConfiguration.DashboardUrl },
                        { "expiry_date", expiryDate.ToString("dd MMM yyy")}
                    }
                }
            });
        }
    }
}