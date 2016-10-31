﻿using System;

namespace SFA.DAS.EAS.Domain.Entities.Account
{
    public class LegalEntity
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string RegisteredAddress { get; set; }

        public DateTime DateOfIncorporation { get; set; }
    }
}