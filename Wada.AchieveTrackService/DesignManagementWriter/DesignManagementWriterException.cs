namespace Wada.AchieveTrackService.DesignManagementWriter
{
    public class DesignManagementWriterException : DomainException
    {
        public DesignManagementWriterException()
        {
        }

        public DesignManagementWriterException(string? message) : base(message)
        {
        }

        public DesignManagementWriterException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}