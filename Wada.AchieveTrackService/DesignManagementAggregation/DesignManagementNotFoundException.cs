namespace Wada.AchieveTrackService.DesignManagementAggregation
{
    public class DesignManagementNotFoundException : DesignManagementAggregationException
    {
        public DesignManagementNotFoundException()
        {
        }

        public DesignManagementNotFoundException(string? message) : base(message)
        {
        }

        public DesignManagementNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}