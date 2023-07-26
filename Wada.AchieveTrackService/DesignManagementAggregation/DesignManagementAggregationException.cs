using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.DesignManagementAggregation
{
    [Serializable]
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

        protected DesignManagementAggregationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}