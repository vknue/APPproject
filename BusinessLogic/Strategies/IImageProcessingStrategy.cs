using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Strategies
{
    public interface IImageProcessingStrategy
    {
        void Apply(Image image);
    }
}
