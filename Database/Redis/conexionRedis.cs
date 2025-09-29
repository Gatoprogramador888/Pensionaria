using StackExchange.Redis;

namespace Redis
{
    public class conexionRedis
    {
        private readonly ConnectionMultiplexer _redis;
        public IDatabase Db => _redis.GetDatabase();

        public conexionRedis(string configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration);
        }
    }
}
