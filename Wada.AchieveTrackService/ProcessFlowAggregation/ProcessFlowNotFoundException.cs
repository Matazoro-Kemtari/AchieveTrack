using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.ProcessFlowAggregation
{
    [Serializable]
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

        protected ProcessFlowNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}