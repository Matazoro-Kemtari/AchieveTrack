using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.WorkingLedgerAggregation
{
    [Serializable]
    public class WorkingLedgerAggregationException : DomainException
    {
        public WorkingLedgerAggregationException()
        {
        }

        public WorkingLedgerAggregationException(string? message) : base(message)
        {
        }

        public WorkingLedgerAggregationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WorkingLedgerAggregationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}