﻿using MediatR;

namespace SFA.DAS.LevyAggregationProvider.Worker.Queries.GetLevyDeclaration
{
    public class GetLevyDeclarationRequest : IAsyncRequest<GetLevyDeclarationResponse>
    {
        public int AccountId { get; set; }
    }
}