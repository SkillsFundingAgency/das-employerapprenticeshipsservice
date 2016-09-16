using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class ViewModelBase
    {
        protected ViewModelBase()
        {
            ErrorDictionary = new Dictionary<string, string>();
        }

        public bool Valid => !ErrorDictionary.Any();

        public Dictionary<string, string> ErrorDictionary { get; set; }

        protected string GetErrorMessage(string propertyName)
        {
            return ErrorDictionary.Any() && ErrorDictionary.ContainsKey(propertyName) ? ErrorDictionary[propertyName] : "";
        }
    }
}