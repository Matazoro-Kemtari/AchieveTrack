namespace Wada.ReadWorkRecordApplication
{
    public class AchieveTrackIOException : Exception
    {
        public AchieveTrackIOException()
        {
        }

        public AchieveTrackIOException(string? message) : base(message)
        {
        }

        public AchieveTrackIOException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}