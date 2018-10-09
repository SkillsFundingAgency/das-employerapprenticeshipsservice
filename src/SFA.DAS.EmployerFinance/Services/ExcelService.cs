using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using SFA.DAS.EmployerFinance.Interfaces;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ExcelService : IExcelService
    {
        public Dictionary<string, string[][]> ReadExcelFile(byte[] fileData)
        {
            var workbookData = new Dictionary<string, string[][]>();

            using (var stream = new MemoryStream(fileData))
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var worksheetData = new List<string[]>();

                        foreach (var row in worksheet.Rows())
                        {
                            var rowData = row.Cells().Select(x => x.Value.ToString()).ToArray();
                            worksheetData.Add(rowData);
                        }
                        
                        workbookData.Add(worksheet.Name, worksheetData.ToArray());
                    }
                }
            }

            return workbookData;
        }

        public byte[] CreateExcelFile(Dictionary<string, string[][]> worksheets)
        {
            if (!worksheets.Any())
            {
                throw new ArgumentException("Cannot create file with empty dictionary", nameof(worksheets));
            }

            using (var stream = new MemoryStream())
            {
                using (var workbook = new XLWorkbook())
                {
                    foreach (var worksheetDetails in worksheets)
                    {
                        var worksheetName = worksheetDetails.Key;
                        var worksheetData = worksheetDetails.Value;

                        using (var worksheet = workbook.Worksheets.Add(worksheetName))
                        {
                            for (var rowIndex = 0; rowIndex < worksheetData.Length; rowIndex++)
                            {
                                for (var columnIndex = 0; columnIndex < worksheetData[rowIndex].Length; columnIndex++)
                                {
                                    worksheet.Cell(rowIndex + 1, columnIndex + 1).Value = worksheetData[rowIndex][columnIndex];
                                }
                            }
                        }
                    }
                    
                    workbook.SaveAs(stream);
                }

                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }
}
