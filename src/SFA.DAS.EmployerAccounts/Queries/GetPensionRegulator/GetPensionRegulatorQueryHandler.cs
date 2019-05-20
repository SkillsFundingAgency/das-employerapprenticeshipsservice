using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.ReferenceData;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator
{
    public class GetOrganisationsQueryHandler : IAsyncRequestHandler<GetPensionRegulatorRequest, GetPensionRegulatorResponse>
    {
        private readonly IValidator<GetPensionRegulatorRequest> _validator;
        private readonly IReferenceDataService _referenceDataService;

        public GetOrganisationsQueryHandler(IValidator<GetPensionRegulatorRequest> validator, IReferenceDataService referenceDataService)
        {
            _validator = validator;
            _referenceDataService = referenceDataService;
        }

        public async Task<GetPensionRegulatorResponse> Handle(GetPensionRegulatorRequest message)
        {
            var valdiationResult = _validator.Validate(message);

            if (!valdiationResult.IsValid())
            {
                throw new InvalidRequestException(valdiationResult.ValidationDictionary);
            }

            var organisations = new List<OrganisationName>
            {
                new OrganisationName {Name = "Name", Code = "Code"},
                new OrganisationName {Name = "Name2", Code = "Code2"}
            };

            if (organisations == null)
            {
                return new GetPensionRegulatorResponse();
            }

            return new GetPensionRegulatorResponse { Organisations = organisations };
        }
    }
}
