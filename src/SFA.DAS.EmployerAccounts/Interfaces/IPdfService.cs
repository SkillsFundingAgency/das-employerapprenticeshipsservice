﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IPdfService
    {
        Task<MemoryStream> SubsituteValuesForPdf(string fileName, Dictionary<string, string> valuesToSubsitute);

        Task<MemoryStream> SubsituteValuesForPdf(string fileName);
    }
}