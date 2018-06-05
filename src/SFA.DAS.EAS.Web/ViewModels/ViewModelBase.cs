using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.ViewModels
{
    [Obsolete]
    public class ViewModelBase
    {
        public string PublicHashedAccountId { get; set; }
        public long UserId { get; set; }
        public bool Valid => !ErrorDictionary.Any();
        public Dictionary<string, string> ErrorDictionary { get; set; }

        protected ViewModelBase()
        {
            ErrorDictionary = new Dictionary<string, string>();
        }

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

        public void AddErrorsFromDictionary(Dictionary<string, string> errorDictionary)
        {
            foreach (var property in errorDictionary.Keys)
            {
                if (ErrorDictionary.ContainsKey(property)) continue;
                if (!errorDictionary[property].Any()) continue;

                var error = errorDictionary[property];
                ErrorDictionary.Add(property, error);
            }
        }
    }
}