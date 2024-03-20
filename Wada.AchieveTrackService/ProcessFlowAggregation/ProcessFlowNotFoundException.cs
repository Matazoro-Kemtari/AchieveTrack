namespace Wada.AchieveTrackService.ProcessFlowAggregation
{
    public class ProcessFlowNotFoundException : ProcessFlowAggregationException
    {
        public ProcessFlowNotFoundException()
        {
        }

        public ProcessFlowNotFoundException(string? message) : base(message)
        {
        }

        public ProcessFlowNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}