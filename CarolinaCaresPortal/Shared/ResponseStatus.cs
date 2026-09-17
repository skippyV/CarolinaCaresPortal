namespace CarolinaCaresPortal.Shared
{
    public class ResponseStatus
    {
        public bool Success { get; set; }

        public string StatusMessage { get; set; }

        public string ObjectId { get; set; }

        public DbInfoDetails DbActionStatus { get; set; }

        public ResponseStatus()
        {
            Success = false;
            DbActionStatus = DbInfoDetails.noChangesMade;
        }
    }

    public enum DbInfoDetails { errorOccurred = -1, noChangesMade = 0, recordUpdated = 1 }
}
