using Newtonsoft.Json;
using Seckill.EntityFrameworkCore.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seckill.EntityFrameworkCore.Modles
{
    public partial class TbSeckillOrder
    {
        [JsonConverter(typeof(MyNumberConverter), NumberConverterShip.Int64)]
        public long Id { get; set; }
        [JsonConverter(typeof(MyNumberConverter), NumberConverterShip.Int64)]
        public long? SeckillId { get; set; }
        public decimal? Money { get; set; }
        public string UserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? PayTime { get; set; }
        public string Status { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverMobile { get; set; }
        public string Receiver { get; set; }
        public string TransactionId { get; set; }
    }
}
