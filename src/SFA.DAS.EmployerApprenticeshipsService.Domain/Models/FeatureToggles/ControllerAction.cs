using System.Configuration;

namespace SFA.DAS.EAS.Domain.Models.FeatureToggles
{
    public class ControllerAction
    {
        private string _controller;
        private string _action;

        public string Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                SetQualifiedName();
            }
        }

        public string Action
        {
            get => _action;
            set
            {
                _action = value;
                SetQualifiedName();
            }
        }

        public string QualifiedName { get; private set; }

        private void SetQualifiedName()
        {
            if (string.IsNullOrWhiteSpace(_controller) || string.IsNullOrWhiteSpace(_action))
            {
                QualifiedName = string.Empty;
            }
            else
            {
                QualifiedName = $"{_controller}.{_action}";
            }
        }
    }
}