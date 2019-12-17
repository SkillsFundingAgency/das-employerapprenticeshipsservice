﻿using System.Configuration;
using System.IO;
using System.Net;
using Aspose.Pdf;
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
                ? "https://localhost:44344/service/CreateLegalAgreement/true"
                : "https://localhost:44344/service/CreateLegalAgreement/false";

            var request = WebRequest.Create(httpsLocalhostServiceCreatelegalagreementTrue);
            request.Credentials = CredentialCache.DefaultCredentials;

            using (var response = (HttpWebResponse)request.GetResponse())
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
                        var legalAgreementSubPdf = withSubFields ? @".\_NonLevy_Agreement_V2_Sub.pdf" : @".\_NonLevy_Agreement_V2.pdf";
                        pdfDocument.Save(legalAgreementSubPdf);
                    }
                }
            }
        }

        public static MemoryStream GetBlobDataFromAzure(string blobContainer, string blobName)
        {
            var storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            var client = storageAccount.CreateCloudBlobClient();

            var container = client.GetContainerReference(blobContainer);

            var blob = container.GetBlobReference(blobName);

            var stream = new MemoryStream();

            blob.DownloadRangeToStream(stream, 0, null);

            return stream;
        }
    }
}