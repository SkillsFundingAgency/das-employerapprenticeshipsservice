using System.Threading.Tasks;

namespace SFA.DAS.EAS.AccountFixupTool.Work
{
    public interface IAdminJob
    {
        Task Fix();
    }
}