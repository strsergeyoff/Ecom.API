using Ecom.API.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Ecom.API.Jobs
{
    [DisallowConcurrentExecution]
    public class LoadReportDetails(IDataRepository dataRepository) : IJob
    {
        public async Task Execute(IJobExecutionContext context) => await dataRepository.LoadReportDetails();
    }
}
