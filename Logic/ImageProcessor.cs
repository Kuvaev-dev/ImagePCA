using ImagePCA.Interfaces;
using System.Drawing;

namespace ImagePCA.Logic
{
    /// <summary>
    /// Клас для обробки зображень.
    /// </summary>
    public class ImageProcessor : IImageProcessor
    {
        /// <summary>
        /// Отримує дані каналів зображення.
        /// </summary>
        public double[,] GetChannelData(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            double[,] channelData = new double[height * width, 3];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int index = y * width + x;
                    channelData[index, 0] = pixelColor.R;
                    channelData[index, 1] = pixelColor.G;
                    channelData[index, 2] = pixelColor.B;
                }
            }

            return channelData;
        }

        /// <summary>
        /// Створює зображення з матриці даних.
        /// </summary>
        public Bitmap CreateImageFromMatrix(double[,] matrix, Bitmap originalImage, int channelIndex)
        {
            int width = originalImage.Width;
            int height = originalImage.Height;
            Bitmap resultImage = new Bitmap(width, height);

            double minVal = double.MaxValue;
            double maxVal = double.MinValue;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, channelIndex] < minVal) minVal = matrix[i, channelIndex];
                if (matrix[i, channelIndex] > maxVal) maxVal = matrix[i, channelIndex];
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int value = (int)((matrix[index, channelIndex] - minVal) / (maxVal - minVal) * 255.0);
                    Color originalColor = originalImage.GetPixel(x, y);
                    Color newColor = Color.FromArgb(
                        channelIndex == 0 ? value : originalColor.R,
                        channelIndex == 1 ? value : originalColor.G,
                        channelIndex == 2 ? value : originalColor.B);
                    resultImage.SetPixel(x, y, newColor);
                }
            }

            return resultImage;
        }

        /// <summary>
        /// Створює зображення з перетворених даних.
        /// </summary>
        public Bitmap CreateImageFromTransformedData(double[,] transformedData, Bitmap originalImage)
        {
            int width = originalImage.Width;
            int height = originalImage.Height;
            Bitmap resultImage = new Bitmap(width, height);

            double[] minVals = new double[3] { double.MaxValue, double.MaxValue, double.MaxValue };
            double[] maxVals = new double[3] { double.MinValue, double.MinValue, double.MinValue };

            for (int i = 0; i < transformedData.GetLength(0); i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (transformedData[i, j] < minVals[j]) minVals[j] = transformedData[i, j];
                    if (transformedData[i, j] > maxVals[j]) maxVals[j] = transformedData[i, j];
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int r = (int)((transformedData[index, 0] - minVals[0]) / (maxVals[0] - minVals[0]) * 255.0);
                    int g = (int)((transformedData[index, 1] - minVals[1]) / (maxVals[1] - minVals[1]) * 255.0);
                    int b = (int)((transformedData[index, 2] - minVals[2]) / (maxVals[2] - minVals[2]) * 255.0);
                    Color newColor = Color.FromArgb(r, g, b);
                    resultImage.SetPixel(x, y, newColor);
                }
            }

            return resultImage;
        }
    }
}