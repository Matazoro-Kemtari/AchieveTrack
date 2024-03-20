namespace Wada.AchieveTrackService.ValueObjects
{
    public class WorkingNumberException : DomainException
    {
        public WorkingNumberException()
        {
        }

        public WorkingNumberException(string? message) : base(message)
        {
        }

        public WorkingNumberException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}