using MediatR;

namespace SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails
{
    public class UpdateOrganisationDetailsRequest : IAsyncRequest<UpdateOrganisationDetailsResponse>
    {
        public long AccountLegalEntityId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
