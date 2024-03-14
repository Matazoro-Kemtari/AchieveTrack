namespace Wada.AchieveTrackService.WorkRecordValidator
{
    public class WorkRecordValidatorException : DomainException
    {
        public WorkRecordValidatorException()
        {
        }

        public WorkRecordValidatorException(string? message) : base(message)
        {
        }

        public WorkRecordValidatorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}