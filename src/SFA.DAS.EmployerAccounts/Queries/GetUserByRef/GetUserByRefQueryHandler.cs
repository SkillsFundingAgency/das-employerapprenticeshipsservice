using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserByRef
{
    public class GetUserByRefQueryHandler : IAsyncRequestHandler<GetUserByRefQuery, GetUserByRefResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IValidator<GetUserByRefQuery> _validator;
        private readonly ILog _logger;

        public GetUserByRefQueryHandler(IUserRepository repository, IValidator<GetUserByRefQuery> validator, ILog logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<GetUserByRefResponse> Handle(GetUserByRefQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            _logger.Debug($"Getting user with ref {message.UserRef}");

            var user = await _repository.GetUserByRef(message.UserRef);
            //todo issue is here - user coming back is null in save and search scenario
            //which results in the below being true and the loop error we see

            if (user == null)
            {
                validationResult.AddError(nameof(message.UserRef), "User does not exist");
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            return new GetUserByRefResponse{ User = user};
        }
    }
}
