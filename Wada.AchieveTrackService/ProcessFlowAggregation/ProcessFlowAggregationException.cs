using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.ProcessFlowAggregation
{
    [Serializable]
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

        protected ProcessFlowAggregationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}