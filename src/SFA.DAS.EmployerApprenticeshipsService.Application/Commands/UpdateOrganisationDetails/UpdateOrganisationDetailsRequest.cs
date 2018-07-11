using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;

namespace SFA.DAS.EAS.Application.Commands.UpdateOrganisationDetails
{
    public class UpdateOrganisationDetailsRequest : IAsyncRequest<UpdateOrganisationDetailsResponse>
    {
        public long AccountLegalEntityId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
    }
}
