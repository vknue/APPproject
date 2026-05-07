using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Strategies
{
    public class ResizeStrategy : IImageProcessingStrategy
    {

        public void Apply(Image image) => image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(500, 500),
            Mode = ResizeMode.Crop
        }));

    }
}
