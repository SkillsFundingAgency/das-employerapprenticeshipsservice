using System.Threading;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

public class GetEmployerAgreementByIdRequestHandler : IRequestHandler<GetEmployerAgreementByIdRequest, GetEmployerAgreementByIdResponse>
{
    private readonly IEmployerAgreementRepository _employerAgreementRepository;
    private readonly IHashingService _hashingService;
    private readonly IValidator<GetEmployerAgreementByIdRequest> _validator;

    public GetEmployerAgreementByIdRequestHandler(
        IEmployerAgreementRepository employerAgreementRepository, 
        IHashingService hashingService, 
        IValidator<GetEmployerAgreementByIdRequest> validator)
    {
        _employerAgreementRepository = employerAgreementRepository;
        _hashingService = hashingService;
        _validator = validator;
    }

    public async Task<GetEmployerAgreementByIdResponse> Handle(GetEmployerAgreementByIdRequest message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);
        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

        var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

        if (agreement == null)
            throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement could not be found" } });

        agreement.HashedAgreementId = message.HashedAgreementId;

        return new GetEmployerAgreementByIdResponse
        {
            EmployerAgreement = agreement
        };
    }
}