using System.Threading.Tasks;

namespace SFA.DAS.EAS.LevyDeclarationProvider.Worker.Providers
{
    interface ILevyDeclaration
    {
        Task Handle();
    }
}