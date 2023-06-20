using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

public class GetEmployerAgreementByIdRequestHandler : IRequestHandler<GetEmployerAgreementByIdRequest, GetEmployerAgreementByIdResponse>
{
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IValidator<GetEmployerAgreementByIdRequest> _validator;

    public GetEmployerAgreementByIdRequestHandler(
        IEmployerAgreementRepository employerAgreementRepository,
        IValidator<GetEmployerAgreementByIdRequest> validator)
    {
        _employerAgreementRepository = employerAgreementRepository;
        _validator = validator;
    }

    public async Task<GetEmployerAgreementByIdResponse> Handle(GetEmployerAgreementByIdRequest message, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(message);
        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var agreement = await _employerAgreementRepository.GetEmployerAgreement(message.AgreementId);

        if (agreement == null)
            throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement could not be found" } });

        return new GetEmployerAgreementByIdResponse
        {
            EmployerAgreement = agreement
        };
    }
}