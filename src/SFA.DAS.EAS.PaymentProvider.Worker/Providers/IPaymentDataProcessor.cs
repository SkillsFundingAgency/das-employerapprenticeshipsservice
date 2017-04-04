using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    public interface IPaymentDataProcessor
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}