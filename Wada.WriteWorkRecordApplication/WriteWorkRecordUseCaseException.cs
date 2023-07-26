using System.Runtime.Serialization;

namespace Wada.WriteWorkRecordApplication
{
    [Serializable]
    public class WriteWorkRecordUseCaseException : Exception
    {
        public WriteWorkRecordUseCaseException()
        {
        }

        public WriteWorkRecordUseCaseException(string? message) : base(message)
        {
        }

        public WriteWorkRecordUseCaseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WriteWorkRecordUseCaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}