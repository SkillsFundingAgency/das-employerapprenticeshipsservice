using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetOrganisations;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class SearchOrganisationOrchestrator : UserVerificationOrchestratorBase
    {
        public SearchOrganisationOrchestrator(IMediator mediator)
            : base(mediator)
        {  }

        public async Task<OrchestratorResponse<List<Organisation>>> SearchOrganisation(string searchTerm)
        {
            var response = new OrchestratorResponse<List<Organisation>>();

            try
            {
                var result = await Mediator.SendAsync(new GetOrganisationsRequest { SearchTerm = searchTerm });
                response.Data = result.Organisations;
            }
            catch (InvalidRequestException ex)
            {
                response.Exception = ex;
                response.FlashMessage = new FlashMessageViewModel().CreateErrorFlashMessageViewModel(ex.ErrorMessages);
                response.Status = HttpStatusCode.BadRequest;
            }

            return response;
        }
    }
}