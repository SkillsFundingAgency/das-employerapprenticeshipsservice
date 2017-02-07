using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class FlashMessageViewModel 
    {
        public FlashMessageViewModel()
        {
            ErrorMessages = new Dictionary<string, string>();
        }
        public string Headline { get; set; }

        public string Message { get; set; }
        public string SubMessage { get; set; }

        public FlashMessageSeverityLevel Severity { get; set; }

        public string SeverityCssClass
        {
            get
            {
                switch (Severity)
                {
                    case FlashMessageSeverityLevel.Success:
                        return "govuk-box-highlight";
                    case FlashMessageSeverityLevel.Error:
                        return "error-summary";
                    case FlashMessageSeverityLevel.Danger:
                        return "panel panel-danger";
                    case FlashMessageSeverityLevel.Info:
                        return "panel panel-info";
                    case FlashMessageSeverityLevel.Warning:
                        return "panel panel-warning";
                    case FlashMessageSeverityLevel.Okay:
                        return "panel panel-okay";
                }
                return "panel panel-info";
            }
        }

        public Dictionary<string,string> ErrorMessages { get; set; }
        public string RedirectButtonMessage { get; set; }
    }

    public enum FlashMessageSeverityLevel
    {
        Success,
        Info,
        Danger,
        Warning,
        Error,
        Okay
    }
}