using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerSchemes
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
            return new GetEmployerSchemesResponse {Schemes = new Schemes {SchemesList = employerSchemes.SchemesList} };
        }
    }
}
