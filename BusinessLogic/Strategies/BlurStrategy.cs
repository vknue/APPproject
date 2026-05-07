using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Strategies
{
    public class BlurStrategy : IImageProcessingStrategy
    {
        public void Apply(Image image) => image.Mutate(x => x.GaussianBlur(5f));
    }
}
