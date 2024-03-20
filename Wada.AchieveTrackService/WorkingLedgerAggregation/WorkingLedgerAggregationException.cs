namespace Wada.AchieveTrackService.WorkingLedgerAggregation
{
    public partial class WorkingLedgerAggregationException : DomainException
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
    }
}