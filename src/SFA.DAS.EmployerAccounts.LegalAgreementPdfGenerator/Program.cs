using System.Text;
using Aspose.Pdf;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.EAS.LegalAgreementPdfGenerator;

class Program
{

    static async Task Main(string[] args)
    {
        await CreatePdfDocument(true);
        await CreatePdfDocument(false);
    }

    private static async Task CreatePdfDocument(bool withSubFields)
    {
        var licenseStream = await GetBlobDataFromAzure("legal-agreements", "Aspose.Total.lic");

        var license = new License();
        licenseStream.Position = 0;
        license.SetLicense(licenseStream);

        var httpsLocalhostServiceCreatelegalagreementTrue = withSubFields
            ? "https://localhost:44344/service/CreateLegalAgreement/true"
            : "https://localhost:44344/service/CreateLegalAgreement/false";

        using var client = new HttpClient();
        using var response = await client.GetAsync(httpsLocalhostServiceCreatelegalagreementTrue);
        using var dataStream = await response.Content.ReadAsStreamAsync();

        using var reader = new StreamReader(dataStream);
        var responseFromServer = await reader.ReadToEndAsync();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(responseFromServer));
        var options = new HtmlLoadOptions(httpsLocalhostServiceCreatelegalagreementTrue);

        // Load HTML file
        var pdfDocument = new Document(stream, options);

        options.PageInfo.IsLandscape = false;
        pdfDocument.Info.Title = "ESFA Agreement";

        // Save output as PDF format
        var legalAgreementSubPdf = withSubFields ? @".\_NonLevy_Agreement_V2_Sub.pdf" : @".\_NonLevy_Agreement_V2.pdf";
        pdfDocument.Save(legalAgreementSubPdf);
    }

    public static async Task<MemoryStream> GetBlobDataFromAzure(string blobContainer, string blobName)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("StorageAccount");
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(blobContainer);
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadAsync();
        var stream = new MemoryStream();
        await response.Value.Content.CopyToAsync(stream);
        stream.Position = 0;

        return stream;
    }
}

