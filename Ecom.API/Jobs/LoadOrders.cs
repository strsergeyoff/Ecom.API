using Ecom.API.Services;
using Quartz;

namespace Ecom.API.Jobs
{
    [DisallowConcurrentExecution]
    public class LoadOrders(IDataRepository dataRepository, IServiceProvider serviceProvider) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await dataRepository.LoadOrders();
            //await dataRepository.LoadReportDetails();
        }
    }
}
