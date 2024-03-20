namespace Wada.AchieveTrackService.EmployeeAggregation
{
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
    }
}