using SFA.DAS.EmployerAccounts.Commands.UnsubscribeProviderEmail;
using SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class HomeOrchestrator
{
    private readonly IMediator _mediator;

    //Required for running tests
    public HomeOrchestrator()
    {
    }

    public HomeOrchestrator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public virtual async Task<OrchestratorResponse<UserAccountsViewModel>> GetUserAccounts(string userId, DateTime? LastTermsAndConditionsUpdate = null)
    {
        var getUserAccountsQueryResponse = await _mediator.Send(new GetUserAccountsQuery
        {
            UserRef = userId
        });
        var getUserInvitationsResponse = await _mediator.Send(new GetNumberOfUserInvitationsQuery
        {
            UserId = userId
        });
        var getUserQueryResponse = await _mediator.Send(new GetUserByRefQuery
        {
            UserRef = userId
        });
        return new OrchestratorResponse<UserAccountsViewModel>
        {
            Data = new UserAccountsViewModel
            {
                Accounts = getUserAccountsQueryResponse.Accounts,
                Invitations = getUserInvitationsResponse.NumberOfInvites,
                TermAndConditionsAcceptedOn = getUserQueryResponse.User.TermAndConditionsAcceptedOn,
                LastTermsAndConditionsUpdate = LastTermsAndConditionsUpdate
            }
        };
    }

    public virtual async Task<OrchestratorResponse<ProviderInvitationViewModel>> GetProviderInvitation(Guid correlationId)
    {
        var getProviderInvitationQueryResponse = await _mediator.Send(new GetProviderInvitationQuery
        {
            CorrelationId = correlationId
        });

        if (getProviderInvitationQueryResponse.Result != null)
        {
            return new OrchestratorResponse<ProviderInvitationViewModel>
            {
                Data = new ProviderInvitationViewModel
                {
                    EmployerEmail = getProviderInvitationQueryResponse.Result.EmployerEmail,
                    EmployerFirstName = getProviderInvitationQueryResponse.Result.EmployerFirstName,
                    EmployerLastName = getProviderInvitationQueryResponse.Result.EmployerLastName,
                    EmployerOrganisation = getProviderInvitationQueryResponse.Result.EmployerOrganisation,
                    Ukprn = getProviderInvitationQueryResponse.Result.Ukprn,
                    Status = getProviderInvitationQueryResponse.Result.Status
                }
            };
        }

        return new OrchestratorResponse<ProviderInvitationViewModel>();
    }

    public virtual async Task Unsubscribe(Guid correlationId)
    {
        await _mediator.Send(new UnsubscribeProviderEmailCommand
        {
            CorrelationId = correlationId
        });
    }

    public virtual async Task SaveUpdatedIdentityAttributes(string userRef, string email, string firstName, string lastName, string correlationId = null)
    {
        await _mediator.Send(new UpsertRegisteredUserCommand
        {
            EmailAddress = email,
            UserRef = userRef,
            LastName = lastName,
            FirstName = firstName,
            CorrelationId = correlationId
        });
    }
    
    public virtual async Task UpdateTermAndConditionsAcceptedOn(string userRef)
    {
        await _mediator.Send(new UpdateTermAndConditionsAcceptedOnCommand
        {
            UserRef = userRef
        });
    }

    public virtual async Task<User> GetUser(string userRef)
    {
        try
        {
            var user = await _mediator.Send(new GetUserByRefQuery
            {
                UserRef = userRef
            });
            return user.User;
        }
        catch (InvalidRequestException e)
        {
            return null;
        }
        
    }
}