namespace Wada.AchieveTrackService.WorkingLedgerAggregation
{
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
    }
}