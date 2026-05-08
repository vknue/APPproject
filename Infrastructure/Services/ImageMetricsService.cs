using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ImageMetricsService
    {
        public int TotalImagesProcessed { get; private set; }
        public int ActiveSessions => Random.Shared.Next(1, 10); 
        public double AverageProcessingTime { get; private set; }

        public void RecordImageProcess(long elapsedMs)
        {
            TotalImagesProcessed++;
            //AverageProcessingTime = (AverageProcessingTime + elapsedMs) / 2;     LO5
            AverageProcessingTime = CalculateNewAverage(AverageProcessingTime, elapsedMs);
        }

        //PURE FUNCTIONS
        public static double CalculateNewAverage(double currentAvg, long newTime)
            => (currentAvg + newTime) / 2;

        public int GetActiveSessions(Func<int, int, int> sessionCalculator)
            => sessionCalculator(1, 10);
    }
}
