namespace Wada.AchieveTrackService.ProcessFlowAggregation
{
    public class ProcessFlowAggregationException : DomainException
    {
        public ProcessFlowAggregationException()
        {
        }

        public ProcessFlowAggregationException(string? message) : base(message)
        {
        }

        public ProcessFlowAggregationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}