using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class FlashMessageViewModel 
    {
        public FlashMessageViewModel()
        {
            ErrorMessages = new Dictionary<string, string>();
        }

        public static FlashMessageViewModel CreateErrorFlashMessageViewModel(Dictionary<string, string> errorMessages)
        {
            return new FlashMessageViewModel
            {
                Headline = "Errors to fix",
                Message = "Check the following details:",
                ErrorMessages = errorMessages,
                Severity = FlashMessageSeverityLevel.Error
            };
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
                    case FlashMessageSeverityLevel.Complete:
                        return "govuk-box-highlight";
                    case FlashMessageSeverityLevel.Success:
                        return "success-summary";
                    case FlashMessageSeverityLevel.Error:
                        return "error-summary";
                    case FlashMessageSeverityLevel.Danger:
                        return "panel panel-danger";
                    case FlashMessageSeverityLevel.Info:
                        return "info-summary";
                    case FlashMessageSeverityLevel.Warning:
                        return "warning-summary";
                    case FlashMessageSeverityLevel.Okay:
                        return "panel panel-border-wide alert-default flash-alert";
                }
                return "panel panel-info";
            }
        }

        public Dictionary<string,string> ErrorMessages { get; set; }
        public string RedirectButtonMessage { get; set; }
        public string HiddenFlashMessageInformation { get; set; }
    }

    public enum FlashMessageSeverityLevel
    {
        Complete,
        Success,
        Info,
        Danger,
        Warning,
        Error,
        Okay
    }
}