using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EAS.Application.Messages
{
    public abstract class ViewModel<T> : ViewModel where T : class
    {
        [Required]
        public T Message { get; set; }

        protected abstract T Map();

        public override void OnActionExecuted()
        {
            if (Message == null)
            {
                Message = Map();
            }
        }
    }

    public abstract class ViewModel
    {
        public abstract void OnActionExecuted();
    }
}