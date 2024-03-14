namespace Wada.AchieveTrackService.EmployeeAggregation
{
    public class EmployeeAggregationException : DomainException
    {
        public EmployeeAggregationException()
        {
        }

        public EmployeeAggregationException(string? message) : base(message)
        {
        }

        public EmployeeAggregationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}