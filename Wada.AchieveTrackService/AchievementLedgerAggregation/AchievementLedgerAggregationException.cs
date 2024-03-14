namespace Wada.AchieveTrackService.AchievementLedgerAggregation
{
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
    }
}