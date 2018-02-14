using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IExcelService
    {
        byte[] CreateExcelFile(Dictionary<string, string[][]> worksheets);
        Dictionary<string, string[][]> ReadExcelFile(byte[] fileData);
    }
}
