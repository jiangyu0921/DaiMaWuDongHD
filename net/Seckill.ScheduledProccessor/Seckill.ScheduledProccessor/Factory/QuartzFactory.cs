using Quartz.Spi;
using Quartz;

namespace Seckill.ScheduledProccessor.Factory
{
    public class QuartzFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public QuartzFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }
        public void ReturnJob(IJob job) { }
    }
}
