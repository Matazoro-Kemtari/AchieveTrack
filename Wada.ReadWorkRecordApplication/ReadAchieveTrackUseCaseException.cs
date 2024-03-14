namespace Wada.ReadWorkRecordApplication
{
    public class ReadAchieveTrackUseCaseException : Exception
    {
        public ReadAchieveTrackUseCaseException()
        {
        }

        public ReadAchieveTrackUseCaseException(string? message) : base(message)
        {
        }

        public ReadAchieveTrackUseCaseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}