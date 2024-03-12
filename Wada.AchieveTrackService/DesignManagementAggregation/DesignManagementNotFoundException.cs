using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.DesignManagementAggregation
{
    [Serializable]
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

        protected DesignManagementNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}