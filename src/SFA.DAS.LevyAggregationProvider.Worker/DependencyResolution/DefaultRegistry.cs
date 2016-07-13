using MediatR;
using SFA.DAS.LevyAggregationProvider.Worker.Providers;
using StructureMap;

namespace SFA.DAS.LevyAggregationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.WithDefaultConventions();
                    //scan.AssemblyContainingType<IEmployerVerificationService>();
                    //scan.AssemblyContainingType<GetUsersQuery>();
                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                });
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<ILevyDeclarationReader>().Use<LevyDeclarationReader>();
            For<ILevyAggregationWriter>().Use<LevyAggregationWriter>();

            For<IMediator>().Use<Mediator>();
        }
    }
}