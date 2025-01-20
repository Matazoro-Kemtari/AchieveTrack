namespace Wada.AchieveTrackService.WorkOrderAggregation
{
    public partial class WorkOrderAggregationException : DomainException
    {
        public WorkOrderAggregationException()
        {
        }

        public WorkOrderAggregationException(string? message) : base(message)
        {
        }

        public WorkOrderAggregationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}