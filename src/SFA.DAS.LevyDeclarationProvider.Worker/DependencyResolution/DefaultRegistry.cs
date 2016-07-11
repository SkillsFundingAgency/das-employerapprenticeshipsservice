using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace SFA.DAS.LevyDeclarationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry()
        {
            For<IPollingMessageReceiver>().Use(()=>new Messaging.FileSystem.FileSystemMessageService(""));
            For<ILevyDeclaration>().Use<LevyDeclaration>();

            AddMediatrRegistrations();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            
            For<IMediator>().Use<Mediator>();
        }
    }
    
}
