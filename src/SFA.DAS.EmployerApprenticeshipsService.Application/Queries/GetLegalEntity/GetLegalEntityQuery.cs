﻿using System.ComponentModel.DataAnnotations;
using MediatR;
using SFA.DAS.Authorization;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntity
{
    public class GetLegalEntityQuery : AccountMessage, IAsyncRequest<GetLegalEntityResponse>
    {
        [Required]
        public long? LegalEntityId { get; set; }
    }
}