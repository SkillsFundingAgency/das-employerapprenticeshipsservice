using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Paye;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerSchemes
{
    public class GetEmployerSchemesHandler : IAsyncRequestHandler<GetEmployerSchemesQuery, GetEmployerSchemesResponse>
    {
        private IEmployerSchemesRepository _employerSchemesRepository;

        public GetEmployerSchemesHandler(IEmployerSchemesRepository employerSchemesRepository)
        {
            _employerSchemesRepository = employerSchemesRepository;
        }

        public async Task<GetEmployerSchemesResponse> Handle(GetEmployerSchemesQuery message)
        {
             var employerSchemes = await _employerSchemesRepository.GetSchemesByEmployerId(message.Id);
            return new GetEmployerSchemesResponse {PayeSchemes = new PayeSchemes {SchemesList = employerSchemes.SchemesList} };
        }
    }
}
