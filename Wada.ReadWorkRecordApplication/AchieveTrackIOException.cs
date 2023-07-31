using System.Runtime.Serialization;

namespace Wada.ReadWorkRecordApplication
{
    [Serializable]
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

        protected AchieveTrackIOException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}