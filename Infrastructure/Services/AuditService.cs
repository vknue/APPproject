using BusinessLogic.Models;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IServiceProvider _serviceProvider;

        public AuditService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task LogAsync(AuditLog log)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.AuditLogs.Add(log);
                await context.SaveChangesAsync();
            }
        }
    }
}
