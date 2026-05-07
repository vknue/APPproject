using BusinessLogic.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AADBDT.Filters
{
    public class AuditFilter : IAsyncActionFilter
    {
        private readonly IAuditService _auditService;

        public AuditFilter(IAuditService auditService) => _auditService = auditService;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next(); 

            var log = new AuditLog
            {
                UserId = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                UserName = context.HttpContext.User.Identity?.Name ?? "Guest",
                Controller = context.RouteData.Values["controller"]?.ToString() ?? "",
                Action = context.RouteData.Values["action"]?.ToString() ?? "",
                Timestamp = DateTime.Now,
                IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            await _auditService.LogAsync(log);
        }
    }
}
