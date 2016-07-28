using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(EmailMessage emailMessage);
    }
}
