using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using NLog;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class PdfService : AzureServiceBase<string>, IPdfService
    {
        
        public override string ConfigurationName => "legal-agreements";
        public sealed override ILogger Logger { get; set; }

        public PdfService(ILogger logger)
        {
            Logger = logger;
        }

        public async Task<MemoryStream> SubsituteValuesForPdf(string fileName)
        {
            var returnValue = await SubsituteValuesForPdf(fileName, new Dictionary<string, string>());
            return returnValue;
        }

        public async Task<MemoryStream> SubsituteValuesForPdf(string fileName, Dictionary<string,string> valuesToSubsitute )
        {
            var pdfStream = await GetBlobDataFromAzure(ConfigurationName, fileName);
            
            await SetPdfLicense();

            var pdfDocument = new Document(pdfStream);

            foreach (var key in valuesToSubsitute)
            {

                var textFragmentAbsorber = new TextFragmentAbsorber(key.Key)
                {
                    TextSearchOptions = new TextSearchOptions(true)
                };

                pdfDocument.Pages[1].Accept(textFragmentAbsorber);

                // Get the extracted text fragments
                var textFragmentCollection = textFragmentAbsorber.TextFragments;

                // Loop through the fragments
                foreach (TextFragment textFragment in textFragmentCollection)
                {
                    // Update text and other properties
                    textFragment.Text = key.Value;
                    textFragment.TextState.FontStyle = FontStyles.Bold;
                }
            }
            
            var streamOutput = new MemoryStream();
            pdfDocument.Save(streamOutput);

            return streamOutput;
        }

        private async Task SetPdfLicense()
        {
            using (var licenseStream = await GetBlobDataFromAzure(ConfigurationName, "Aspose.Total.lic"))
            {
                var license = new License();
                licenseStream.Position = 0;
                license.SetLicense(licenseStream);
            }
        }
    }

 
}
