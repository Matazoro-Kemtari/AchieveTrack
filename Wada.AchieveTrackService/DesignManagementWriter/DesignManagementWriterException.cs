using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.DesignManagementWriter
{
    [Serializable]
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

        protected DesignManagementWriterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}