using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using BusinessLogic.Strategies;
using SixLabors.ImageSharp.Processing;

namespace APPTesting
{
    public class PhotoStrategyTesting
    {
        /*
         * Check size ios 500x500 after applying
         */
        [Fact]
        public void Apply_ResizesImageToSquare_WhenCalled()
        {
            using var image = new Image<Rgba32>(1000, 800);
            var strategy = new ResizeStrategy(); 

            strategy.Apply(image);

            Assert.Equal(500, image.Width);
            Assert.Equal(500, image.Height);
        }

        /*
         * Check if photo not the same after applying inverting filter
         */
        [Fact]
        public void InvertStrategy_ShouldChangeImagePixels()
        {
            using var imageBefore = new Image<Rgba32>(1, 1);

            imageBefore[0, 0] = Color.White;

            var imageAfter = imageBefore.Clone(); 
            var strategy = new InvertStrategy();

            strategy.Apply(imageAfter);

            var resultPixelBefore = imageBefore[0, 0];
            var resultPixelAfter = imageAfter[0, 0];
            Assert.NotEqual(resultPixelBefore, resultPixelAfter);
        }
    }
}
