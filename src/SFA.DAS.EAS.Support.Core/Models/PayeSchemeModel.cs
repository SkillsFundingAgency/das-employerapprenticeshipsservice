using SFA.DAS.EAS.Account.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Support.Core.Models
{
    public class PayeSchemeModel
    {
        public string DasAccountId { get; set; }
        public string Ref { get; set; }
        public string Name { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public string HashedPayeRef { get; set; }
        public string ObscuredPayeRef { get; set; }
        public string PayeRefWithOutSlash {
            get
            {
                return Ref.Replace("/", string.Empty);
            }
        }
    }
}
