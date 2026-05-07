using BusinessLogic.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Factories
{
    public static class FilterFactory
    {
        public static IImageProcessingStrategy GetStrategy(string filter) => filter.ToLower() switch
        {
            "sepia" => new SepiaStrategy(),
            "blur" => new BlurStrategy(),
            "invert" => new InvertStrategy(),
            "resize" => new ResizeStrategy(),
            _ => null
        };
    }
}
