using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.WorkRecordValidator
{
    [Serializable]
    public class WorkRecordValidatorException : Exception
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

        protected WorkRecordValidatorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}