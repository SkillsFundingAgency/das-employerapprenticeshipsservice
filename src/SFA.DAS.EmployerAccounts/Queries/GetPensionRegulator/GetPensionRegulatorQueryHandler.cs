using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator
{
    public class GetPensionRegulatorQueryHandler : IAsyncRequestHandler<GetPensionRegulatorRequest, GetPensionRegulatorResponse>
    {
        private readonly IValidator<GetPensionRegulatorRequest> _validator;
        private readonly IPensionRegulatorService _pensionRegulatorService;

        public GetPensionRegulatorQueryHandler(IValidator<GetPensionRegulatorRequest> validator, IPensionRegulatorService pensionRegulatorService)
        {
            _validator = validator;
            _pensionRegulatorService = pensionRegulatorService;
        }

        public async Task<GetPensionRegulatorResponse> Handle(GetPensionRegulatorRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var organisations = await _pensionRegulatorService.GetOrgansiationsByPayeRef(message.PayeRef);

            //var organisations = new List<Organisation>
            //{
            //    new Organisation
            //    {
            //        Name = "Boots PLC",
            //        UniqueIdentity = 1234,
            //        Type = OrganisationType.PensionsRegulator,
            //        Address = new Address
            //        {
            //            Line1 = "1 Boots Road",
            //            Line2 = "2 Boots Place",
            //            Line3 = "3 Boots Town",
            //            Line4 = "4 Boots County",
            //            Line5 = "5 Boots Country",
            //            Postcode = "Boots Postcode"
            //        },
            //        Status = "Active"
            //    },
            //    new Organisation
            //    {
            //        Name = "Maplin LTD",
            //        UniqueIdentity = 4678,
            //        Type = OrganisationType.PensionsRegulator,
            //        Address = new Address
            //        {
            //            Line1 = "1 Maplin Road",
            //            Line2 = "2 Maplin Place",
            //            Line3 = "3 Maplin Town",
            //            Line4 = "4 Maplin County",
            //            Line5 = "5 Maplin Country",
            //            Postcode = "Maplin Postcode"
            //        },
            //        Status = "Active"
            //    }
            //};

            if (organisations == null)
            {
                return new GetPensionRegulatorResponse();
            }

            return new GetPensionRegulatorResponse { Organisations = organisations };
        }
    }
}
