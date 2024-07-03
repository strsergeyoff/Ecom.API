using Ecom.API.Services;
using Quartz;

namespace Ecom.API.Jobs
{
    [DisallowConcurrentExecution]
    public class LoadIncomes(IDataRepository dataRepository) : IJob
    {
        public async Task Execute(IJobExecutionContext context) => await dataRepository.LoadIncomes();
    }
}