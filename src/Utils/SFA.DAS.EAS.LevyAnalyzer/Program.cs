using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using SFA.DAS.EAS.LevyAnalyser.CommandLine;
using SFA.DAS.EAS.LevyAnalyser.Commands;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using StructureMap;

namespace SFA.DAS.EAS.LevyAnalyser
{
    internal class Program
    {
        private readonly IContainer _container;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AnalyseCommandLine>(args)
                .WithParsed(qcla => new Program().RunQuery(qcla));
        }


        public Program()
        {
            _container = IoC.IoC.InitialiseIoC();
        }

        private void RunQuery(AnalyseCommandLine args)
        {
            SetConfigOverrides<AnalyzeCommandConfig>(config => config.AccountIds = args.AccountIds);
            RunCommand<AnalyzeCommand>();
        }

        private void RunCommand<TCommand>() where TCommand : ICommand
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var task = StartCommand<TCommand>(cancellationTokenSource.Token);

            WaitForCommandToCompleteOrCancel(task, cancellationTokenSource);
        }

        private void SetConfigOverrides<TConfigType>(Action<TConfigType> setter) where TConfigType : class, new()
        {
            SetConfigOverrides(setter, true);
        }

        private void SetConfigOverrides<TConfigType>(Action<TConfigType> setter, bool setCondition) where TConfigType : class, new()
        {
            if (setCondition)
            {
                var configProvider = _container.GetInstance<IConfigProvider>();
                var config = configProvider.Get<TConfigType>();
                setter(config);
            }
        }

        private Task StartCommand<TCommand>(CancellationToken cancellationToken) where TCommand : ICommand
        {
            var command = _container.GetInstance<TCommand>();
            var task = command.DoAsync(cancellationToken);
            Console.WriteLine("Task executing - waiting for it to finish");
            return task;
        }

        private void WaitForCommandToCompleteOrCancel(Task commandTask, CancellationTokenSource cancellationTokenSource)
        {
            StartWaitingForManualCancelAsync(cancellationTokenSource);

            commandTask.Wait(cancellationTokenSource.Token);

            cancellationTokenSource.Cancel(false);
        }

        private Task StartWaitingForManualCancelAsync(CancellationTokenSource cancellationTokenSource)
        {
            return Task.Run((Action)(() =>
            {
                Console.WriteLine("press escape to cancel command");
                while (Console.ReadKey(true).Key != ConsoleKey.Escape && !cancellationTokenSource.IsCancellationRequested)
                    Console.WriteLine("Key ignored - press escape to quit");
                cancellationTokenSource.Cancel(false);
            }), cancellationTokenSource.Token);
        }
    }
}
