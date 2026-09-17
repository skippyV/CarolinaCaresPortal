namespace CarolinaCaresPortal.Data
{
    public class MongoDbConfig
    {
        public required string CarolinaCaresCustomersDbName { get; init; }
       
        public required string Host { get; init; }
        public int Port { get; init; }
        public string MongoDbConnectionString => $"mongodb://{Host}:{Port}/{CarolinaCaresCustomersDbName}";

    }
}
