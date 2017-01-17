using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.PublicSectorDataJsonFormatter.Model;

namespace SFA.DAS.EAS.PublicSectorDataJsonFormatter
{
    class Program
    {
        static void Main(string[] args)
        {
            var directory = Directory.GetCurrentDirectory();

            var model = new OrganisationList();

            ImportFile(Path.Combine(directory, "ons"), DataSource.Ons, model);
            ImportFile(Path.Combine(directory, "nhs"), DataSource.Nhs, model);
            ImportFile(Path.Combine(directory, "police"), DataSource.Police, model);

            ExportFile(Path.Combine(directory, "output.json"), model);
        }

        private static void ImportFile(string filename, DataSource source, OrganisationList model)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Unable to find {source} file at {filename}");
                return;
            }

            string[] lines;

            try
            {
                lines = File.ReadAllLines(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred importing {source} file at {filename}: {ex.Message}");
                return;
            }

            foreach (var line in lines)
            {
                var organisation = new Organisation
                {
                    Name = line,
                    Source = source
                };

                model.OrganisationNames.Add(organisation);
            }

            Console.WriteLine($"Imported {lines.Length} {source} records from {filename}");
        }

        private static void ExportFile(string filename, OrganisationList model)
        {
            try
            {
                using (var fs = File.Open(filename, FileMode.Create))
                using (var sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;

                    var serializer = new JsonSerializer();
                    serializer.Serialize(jw, model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred exporting data to {filename}: {ex.Message}");
                return;
            }

            Console.WriteLine($"Exported {model.OrganisationNames.Count} records");
        }

    }
}
