using System.Drawing;

namespace ImagePCA.Interfaces
{
    public enum ColorChannel
    {
        R,
        G,
        B
    }

    public interface IImageProcessor
    {
        Bitmap ProcessImage(Bitmap image, ColorChannel channel);
    }
}
