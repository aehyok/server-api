using Lychee.Core.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Lychee.Sample
{
    public class SeedDataStartupTask : IStartupTask
    {
        private readonly ILogger logger;

        public SeedDataStartupTask(ILogger<SeedDataStartupTask> logger)
        {
            this.logger = logger;
        }

        public int Order { get; set; } = 1;

        public async Task ExecuteAsync()
        {
            this.logger.LogInformation("Hello");
            await Task.CompletedTask;
        }
    }
}