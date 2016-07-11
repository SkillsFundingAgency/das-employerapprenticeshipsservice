using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.LevyDeclarationProvider.Worker.Providers;
using StructureMap.Configuration.DSL;

namespace SFA.DAS.LevyDeclarationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry()
        {
            
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
