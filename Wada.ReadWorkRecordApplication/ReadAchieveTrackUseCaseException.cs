using System.Runtime.Serialization;

namespace Wada.ReadWorkRecordApplication
{
    [Serializable]
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

        protected ReadAchieveTrackUseCaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}