namespace Wada.WriteWorkRecordApplication
{
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
    }
}