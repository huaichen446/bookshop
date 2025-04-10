using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BulkyBookWeb.BackgroundServices
{
    public class ProductRelationsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ProductRelationsBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        unitOfWork.Recommendation.GenerateProductRelations();
                    }
                }
                catch (Exception ex)
                {
                    // 记录异常但不中断服务
                    Console.WriteLine($"Error in ProductRelationsBackgroundService: {ex.Message}");
                }

                // 每天执行一次
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

    }
}
