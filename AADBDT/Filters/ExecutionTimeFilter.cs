using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace AADBDT.Filters
{
    public class ExecutionTimeFilter : IActionFilter
    {
        private Stopwatch _stopwatch;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var elapsed = _stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Action {context.ActionDescriptor.DisplayName} took {elapsed}ms");
        }
    }
}
