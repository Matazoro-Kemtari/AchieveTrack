namespace Wada.AchieveTrackService.DesignManagementAggregation
{
    public class DesignManagementAggregationException : DomainException
    {
        public DesignManagementAggregationException()
        {
        }

        public DesignManagementAggregationException(string? message) : base(message)
        {
        }

        public DesignManagementAggregationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}