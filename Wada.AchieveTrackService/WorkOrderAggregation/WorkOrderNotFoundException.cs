namespace Wada.AchieveTrackService.WorkOrderAggregation
{
    public class WorkOrderNotFoundException : WorkOrderAggregationException
    {
        public WorkOrderNotFoundException()
        {
        }

        public WorkOrderNotFoundException(string? message) : base(message)
        {
        }

        public WorkOrderNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}