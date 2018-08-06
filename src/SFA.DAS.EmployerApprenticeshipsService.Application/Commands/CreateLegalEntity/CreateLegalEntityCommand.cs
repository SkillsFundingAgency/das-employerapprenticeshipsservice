﻿using System;
using MediatR;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommand : IAsyncRequest<CreateLegalEntityCommandResponse>
    {
        public string HashedAccountId { get; set; }
        public string Code { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public byte? PublicSectorDataSource { get; set; }
        public string Sector { get; set; }
        public byte Source { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        //TODO: the two signed fields don't appear to be used?
        public bool SignAgreement { get; set; }

        public DateTime SignedDate { get; set; }

        public string ExternalUserId { get; set; }
    }
}
