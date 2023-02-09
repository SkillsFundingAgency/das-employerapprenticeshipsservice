using System.IO;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.EmployerAccounts.Services;

public class PdfService : AzureServiceBase<string>, IPdfService
{
    public override string ConfigurationName => "legal-agreements";
    public sealed override ILogger Logger { get; set; }

    public PdfService(
        ILogger logger,
        IAutoConfigurationService autoConfigurationService, 
        IConfiguration configuration) 
        : base(autoConfigurationService, configuration)
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
        var pdfStream = await StreamDataFromBlobStorage(ConfigurationName, fileName);
            
        await SetPdfLicense();

        var pdfDocument = new Document(pdfStream);

        foreach (var key in valuesToSubsitute)
        {

            var textFragmentAbsorber = new TextFragmentAbsorber($"__{key.Key}__")
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
        using (var licenseStream = await StreamDataFromBlobStorage(ConfigurationName, "Aspose.Total.lic"))
        {
            var license = new License();
            licenseStream.Position = 0;
            license.SetLicense(licenseStream);
        }
    }
}