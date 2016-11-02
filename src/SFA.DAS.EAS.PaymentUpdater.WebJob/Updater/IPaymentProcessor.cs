using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob.Updater
{
    public interface IPaymentProcessor
    {
        Task RunUpdate();
    }
}