using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain
{
    public class TaskDto
    {
        public string Type { get; set; }
        public string OwnerId { get; set; }
        public int ItemsDueCount { get; set; }
    }
}
