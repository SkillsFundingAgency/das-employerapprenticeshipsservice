namespace SFA.DAS.EAS.Web.Validators.Messages
{
    public struct ValidationMessage
    {
        public ValidationMessage(string text, string errorCode)
        {
            ErrorCode = errorCode;
            Text = text;
        }

        public string Text { get; }

        public string ErrorCode { get; }
    }
}