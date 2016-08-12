using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ApplicationInsights.DataContracts;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class FlashMessageViewModel
    {
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
                    case FlashMessageSeverityLevel.Danger:
                        return "panel panel-danger";
                    case FlashMessageSeverityLevel.Info:
                        return "panel panel-info";
                    case FlashMessageSeverityLevel.Warning:
                        return "panel panel-warning";
                }
                return "panel panel-info";
            }
        }

        public string RedirectButtonMessage { get; set; }
    }

    public enum FlashMessageSeverityLevel
    {
        Success,
        Info,
        Danger,
        Warning
    }
}