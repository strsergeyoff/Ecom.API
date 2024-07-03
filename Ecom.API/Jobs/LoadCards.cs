using Ecom.API.Services;
using Quartz;

namespace Ecom.API.Jobs
{
    [DisallowConcurrentExecution]
    public class LoadCards(IDataRepository dataRepository) : IJob
    {
        public async Task Execute(IJobExecutionContext context) => await dataRepository.CardsWildberries();
    }
}