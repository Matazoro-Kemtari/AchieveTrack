using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.WorkingLedgerAggregation
{
    [Serializable]
    public class WorkingLedgerNotFoundException : WorkingLedgerAggregationException
    {
        public WorkingLedgerNotFoundException()
        {
        }

        public WorkingLedgerNotFoundException(string? message) : base(message)
        {
        }

        public WorkingLedgerNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WorkingLedgerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}