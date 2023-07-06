using System.Threading;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.SendNotification;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerAccounts.Commands.CreateInvitation;

public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMediator _mediator;
    private readonly EmployerAccountsConfiguration _employerApprenticeshipsServiceConfiguration;
    private readonly IValidator<CreateInvitationCommand> _validator;
    private readonly IUserAccountRepository _userRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly bool _isProdEnvironment;

    public CreateInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator,
        EmployerAccountsConfiguration employerApprenticeshipsServiceConfiguration, IValidator<CreateInvitationCommand> validator,
        IUserAccountRepository userRepository, IEventPublisher eventPublisher, IConfiguration configuration)
    {
        _invitationRepository = invitationRepository;
        _membershipRepository = membershipRepository;
        _mediator = mediator;
        _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
        _validator = validator;
        _userRepository = userRepository;
        _eventPublisher = eventPublisher;
        _isProdEnvironment = configuration["ResourceEnvironmentName"]
            .Equals("prd", StringComparison.CurrentCultureIgnoreCase);
    }

    public async Task<Unit> Handle(CreateInvitationCommand message, CancellationToken cancellationToken)
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
                Role = message.RoleOfPersonBeingInvited,
                Status = InvitationStatus.Pending,
                ExpiryDate = expiryDate
            });
        }
        else
        {
            existingInvitation.Name = message.NameOfPersonBeingInvited;
            existingInvitation.Role = message.RoleOfPersonBeingInvited;
            existingInvitation.Status = InvitationStatus.Pending;
            existingInvitation.ExpiryDate = expiryDate;

            await _invitationRepository.Resend(existingInvitation);

            invitationId = existingInvitation.Id;
        }

        var existingUser = await _userRepository.Get(message.EmailOfPersonBeingInvited);

        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "CREATED",
                Description = $"Member {message.EmailOfPersonBeingInvited} added to account {caller.AccountId} as {message.RoleOfPersonBeingInvited}",
                ChangedProperties = new List<PropertyUpdate>
                {

                    PropertyUpdate.FromString("AccountId",caller.AccountId.ToString()),
                    PropertyUpdate.FromString("Email",message.EmailOfPersonBeingInvited),
                    PropertyUpdate.FromString("Name",message.NameOfPersonBeingInvited),
                    PropertyUpdate.FromString("Role",message.RoleOfPersonBeingInvited.ToString()),
                    PropertyUpdate.FromString("Status",InvitationStatus.Pending.ToString()),
                    PropertyUpdate.FromDateTime("ExpiryDate",expiryDate)
                },
                RelatedEntities = new List<AuditEntity> { new AuditEntity { Id = caller.AccountId.ToString(), Type = "Account" } },
                AffectedEntity = new AuditEntity { Type = "Invitation", Id = invitationId.ToString() }
            }
        }, cancellationToken);

        var govLoginExistingUser = "11cb4eb4-c22a-47c7-aa26-1074da25ff4d";//test ---- 3c285db3-164c-4258-9180-f2d42723e155 prod

        var templateId = existingUser?.UserRef != null 
            ?  "InvitationExistingUser" : "InvitationNewUser";
        if (_employerApprenticeshipsServiceConfiguration.UseGovSignIn)
        {
            if (_isProdEnvironment)
            {
                templateId = existingUser?.UserRef != null 
                    ?  "3c285db3-164c-4258-9180-f2d42723e155" : "6b6b46cc-4a5f-4985-8626-ed239af11d71";
            }
            else
            {
                templateId = existingUser?.UserRef != null 
                    ?  "11cb4eb4-c22a-47c7-aa26-1074da25ff4d" : "2bb7da99-2542-4536-9c15-4eb3466a99e3";
            }
        }
        
        await _mediator.Send(new SendNotificationCommand
        {
            Email = new Email
            {
                RecipientsAddress = message.EmailOfPersonBeingInvited,
                TemplateId = templateId,
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
        }, cancellationToken);

        var callerExternalUserId = caller.UserRef;

        await PublishUserInvitedEvent(caller.AccountId, message.NameOfPersonBeingInvited, caller.FullName(), callerExternalUserId);

        return Unit.Value;
    }

    private Task PublishUserInvitedEvent(long accountId, string personInvited, string invitedByUserName, Guid invitedByUserRef)
    {
        return _eventPublisher.Publish(new InvitedUserEvent
        {
            AccountId = accountId,
            PersonInvited = personInvited,
            UserName = invitedByUserName,
            UserRef = invitedByUserRef,
            Created = DateTime.UtcNow
        });
    }
}