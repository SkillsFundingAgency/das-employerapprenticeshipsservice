using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Commands.CreateLevyAggregation
{
    public class CreateLevyAggregationCommandHandler : AsyncRequestHandler<CreateLevyAggregationCommand>
    {
        private readonly IAggregationRepository _aggregationRepository;

        public CreateLevyAggregationCommandHandler(IAggregationRepository aggregationRepository)
        {
            if (aggregationRepository == null)
                throw new ArgumentNullException(nameof(aggregationRepository));
            _aggregationRepository = aggregationRepository;
        }

        protected override async Task HandleCore(CreateLevyAggregationCommand message)
        {
            //TODO: Paging [Not Sprint 1]

            var json = JsonConvert.SerializeObject(message.Data);

            await _aggregationRepository.Update(message.Data.AccountId, 1, json);
        }
    }
}