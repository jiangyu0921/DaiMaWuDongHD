using CSRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seckill.Common.Helper
{
    public class CSRedisHelper
    {
        private readonly CSRedisClient _client;
        public CSRedisHelper()
        {
            this._client = new CSRedisClient("127.0.0.1:6379,defaultDatabase=1");
            RedisHelper.Initialization(_client);
        }

        public CSRedisClient Client()
        {
            return _client;
        }
    }
}
