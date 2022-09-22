namespace Seckill.API.DTO
{
    public partial class TbSeckillGoodDTO
    {
        public string Id { get; set; }
        public long? SpuId { get; set; }
        public long? SkuId { get; set; }
        public string Name { get; set; }
        public string SmallPic { get; set; }
        public decimal? Price { get; set; }
        public decimal? CostPrice { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? CheckTime { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Num { get; set; }
        public int? StockCount { get; set; }
        public string Introduction { get; set; }
    }
}
