using System;
using System.Collections.Generic;
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
            AverageProcessingTime = (AverageProcessingTime + elapsedMs) / 2;
        }
    }
}
