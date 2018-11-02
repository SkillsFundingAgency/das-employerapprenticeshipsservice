using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.LevyAnalyser.Config;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;

namespace SFA.DAS.EAS.LevyAnalyser.ResultSavers
{
    public class FileResultSaver : IResultSaver
    {
        private readonly IConfigProvider _configProvider;

        public FileResultSaver(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public Task SaveAsync<TResult>(TResult results)
        {
            var config = _configProvider.Get<ResultFileSaverConfig>();

            var fileName = System.IO.Path.Combine(config.FolderName, $"{typeof(TResult).Name}_{DateTime.Now:yyyyMMdd_HHmmss}.json");

            var json = JsonConvert.SerializeObject(results);

            System.IO.File.WriteAllText(fileName, json);

            Console.WriteLine($"Saved output to {fileName}");

            return Task.CompletedTask;
        }
    }
}
