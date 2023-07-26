using System.Runtime.Serialization;

namespace Wada.AchieveTrackService.AchievementLedgerAggregation
{
    [Serializable]
    public class AchievementLedgerAggregationException : DomainException
    {
        public AchievementLedgerAggregationException()
        {
        }

        public AchievementLedgerAggregationException(string? message) : base(message)
        {
        }

        public AchievementLedgerAggregationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AchievementLedgerAggregationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}