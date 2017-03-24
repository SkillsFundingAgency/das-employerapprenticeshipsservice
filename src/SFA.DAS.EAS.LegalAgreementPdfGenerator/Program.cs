using System.IO;
using System.Net;
using Aspose.Pdf;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace SFA.DAS.EAS.LegalAgreementPdfGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            CreatePdfDocument(true);
            CreatePdfDocument(false);
        }

        private static void CreatePdfDocument(bool withSubFields)
        {

            
            using (var licenseStream = GetBlobDataFromAzure("legal-agreements", "Aspose.Total.lic"))
            {
                var license = new License();
                licenseStream.Position = 0;
                license.SetLicense(licenseStream);
            }


            // Create a request for the URL.
            var httpsLocalhostServiceCreatelegalagreementTrue = withSubFields 
                ? "https://localhost/service/CreateLegalAgreement/true" 
                : "https://localhost/service/CreateLegalAgreement/false";
            
            var request = WebRequest.Create(httpsLocalhostServiceCreatelegalagreementTrue);
            request.Credentials = CredentialCache.DefaultCredentials;

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseFromServer));
                        var options = new HtmlLoadOptions(httpsLocalhostServiceCreatelegalagreementTrue);


                        // Load HTML file
                        var pdfDocument = new Document(stream, options);

                        options.PageInfo.IsLandscape = false;
                        pdfDocument.Info.Title = "SFA Agreement";
                        
                        // Save output as PDF format
                        var legalAgreementSubPdf = withSubFields ? @".\_Agreement_V1_Sub.pdf" : @".\_Agreement_V1.pdf";
                        pdfDocument.Save(legalAgreementSubPdf);
                    }
                        
                }
                    
            }
            
        }
        public static MemoryStream GetBlobDataFromAzure(string blobContainer, string blobName)
        {
            
            var storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var client = storageAccount.CreateCloudBlobClient();

            var container = client.GetContainerReference(blobContainer);

            var blob = container.GetBlobReference(blobName);

            var stream = new MemoryStream();

            blob.DownloadRangeToStream(stream, 0, null);

            return stream;
            
        }
    }
}
