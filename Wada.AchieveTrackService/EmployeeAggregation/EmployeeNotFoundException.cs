using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.EmployeeAggregation
{
    [Serializable]
    public class EmployeeNotFoundException : EmployeeAggregationException
    {
        public EmployeeNotFoundException()
        {
        }

        public EmployeeNotFoundException(string? message) : base(message)
        {
        }

        public EmployeeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EmployeeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}