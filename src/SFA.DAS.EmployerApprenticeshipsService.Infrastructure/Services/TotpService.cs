using System;
using System.Globalization;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using Simonbu11.Otp;
using Simonbu11.Otp.Totp;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class TotpService : ITotpService
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public TotpService(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetCode(string timeValue = "")
        {
            var generator = new HmacSha512TotpGenerator(new TotpGeneratorSettings
            {
                SharedSecret = OtpSharedSecret.FromBase32String(_configuration.Hmrc.OgdSecret)
            });
            
            var time = string.IsNullOrEmpty(timeValue)
                ? DateTime.UtcNow
                : DateTime.ParseExact(timeValue, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None);
            var result = generator.Generate(time);

            return result;
        }
        
    }
}
