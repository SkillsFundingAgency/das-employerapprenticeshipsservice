using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Models
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

        public void AddErrorsFromModelState(ModelStateDictionary modelStateDictionary)
        {
            foreach (var property in modelStateDictionary.Keys)
            {
                if (ErrorDictionary.ContainsKey(property)) continue;
                if (!modelStateDictionary[property].Errors.Any()) continue;

                var error = modelStateDictionary[property].Errors.First();
                ErrorDictionary.Add(property, error.ErrorMessage);
            }
        }
    }
}