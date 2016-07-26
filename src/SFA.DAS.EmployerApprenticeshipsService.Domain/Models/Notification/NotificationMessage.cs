using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification
{
    public class NotificationMessage
    {
        public string UserId { get; set; }
        public DateTime DateTime { get; set; }
        public bool ForceFormat { get; set; }
        public string TemplatedId { get; set; }
        public string Data { get; set; }
        public MessageFormat MessageFormat { get; set; }
    }
}
