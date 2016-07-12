using System.Threading.Tasks;

namespace SFA.DAS.LevyDeclarationProvider.Worker.Providers
{
    interface ILevyDeclaration
    {
        Task Handle();
    }
}