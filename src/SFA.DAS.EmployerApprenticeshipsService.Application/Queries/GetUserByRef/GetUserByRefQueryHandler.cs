﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetUserByRef
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

            if (user == null)
            {
                validationResult.AddError(nameof(message.UserRef), "User does not exist");
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            return new GetUserByRefResponse{ User = user};
        }
    }
}
