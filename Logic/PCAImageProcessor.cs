using ImagePCA.Interfaces;
using System;
using System.Drawing;

namespace ImagePCA.Logic
{
    /// <summary>
    /// Реалізація стратегії обробки зображень за допомогою PCA (метод головних компонент).
    /// </summary>
    public class PCAImageProcessor : IImageProcessor
    {
        /// <summary>
        /// Обробляє задане зображення, застосовуючи метод головних компонент (PCA).
        /// </summary>
        /// <param name="originalImage">Оригінальне зображення у форматі <see cref="Bitmap"/>.</param>
        /// <returns>Оброблене зображення у форматі <see cref="Bitmap"/>.</returns>
        public Bitmap ProcessImage(Bitmap originalImage, ColorChannel channel)
        {
            var pixels = ImageToPixels(originalImage, channel);  // Измененный метод
            var meanVector = CalculateMean(pixels);
            var centeredPixels = CenterData(pixels, meanVector);
            var covarianceMatrix = CalculateCovarianceMatrix(centeredPixels);
            var eigenVectors = FindEigenVectors(covarianceMatrix);
            var transformedPixels = ApplyTransformation(centeredPixels, eigenVectors);
            return PixelsToImage(transformedPixels, originalImage.Width, originalImage.Height, channel);  // Измененный метод
        }

        /// <summary>
        /// Перетворює зображення у двовимірний масив пікселів.
        /// </summary>
        /// <param name="image">Зображення, яке потрібно перетворити.</param>
        /// <returns>Двовимірний масив пікселів, де кожен піксель представлений RGB-значеннями.</returns>
        private double[,] ImageToPixels(Bitmap image, ColorChannel channel)
        {
            double[,] pixels = new double[image.Width * image.Height, 1];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int index = y * image.Width + x;

                    switch (channel)
                    {
                        case ColorChannel.R:
                            pixels[index, 0] = pixel.R;
                            break;
                        case ColorChannel.G:
                            pixels[index, 0] = pixel.G;
                            break;
                        case ColorChannel.B:
                            pixels[index, 0] = pixel.B;
                            break;
                    }
                }
            }
            return pixels;
        }

        /// <summary>
        /// Обчислює середнє значення для кожного каналу кольору пікселів.
        /// </summary>
        /// <param name="pixels">Двовимірний масив пікселів.</param>
        /// <returns>Масив середніх значень для кожного каналу кольору.</returns>
        private double[] CalculateMean(double[,] pixels)
        {
            int rowCount = pixels.GetLength(0);
            int colCount = pixels.GetLength(1);
            double[] mean = new double[colCount];

            for (int i = 0; i < colCount; i++)
            {
                double sum = 0;
                for (int j = 0; j < rowCount; j++)
                {
                    sum += pixels[j, i];
                }
                mean[i] = sum / rowCount;
            }

            return mean;
        }

        /// <summary>
        /// Центрує дані, віднімаючи середнє значення від кожного пікселя.
        /// </summary>
        /// <param name="pixels">Двовимірний масив пікселів.</param>
        /// <param name="mean">Масив середніх значень для кожного каналу кольору.</param>
        /// <returns>Центровані пікселі у вигляді двовимірного масиву.</returns>
        private double[,] CenterData(double[,] pixels, double[] mean)
        {
            int rowCount = pixels.GetLength(0);
            int colCount = pixels.GetLength(1);
            double[,] centeredPixels = new double[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    centeredPixels[i, j] = pixels[i, j] - mean[j];
                }
            }

            return centeredPixels;
        }

        /// <summary>
        /// Обчислює матрицю ковариації для центрованих пікселів.
        /// </summary>
        /// <param name="centeredPixels">Центровані пікселі у вигляді двовимірного масиву.</param>
        /// <returns>Матриця ковариації.</returns>
        private double[,] CalculateCovarianceMatrix(double[,] centeredPixels)
        {
            int rowCount = centeredPixels.GetLength(0);
            int colCount = centeredPixels.GetLength(1);
            double[,] covarianceMatrix = new double[colCount, colCount];

            for (int i = 0; i < colCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < rowCount; k++)
                    {
                        sum += centeredPixels[k, i] * centeredPixels[k, j];
                    }
                    covarianceMatrix[i, j] = sum / (rowCount - 1);
                }
            }

            return covarianceMatrix;
        }

        /// <summary>
        /// Знаходить власні вектори матриці ковариації.
        /// </summary>
        /// <param name="covarianceMatrix">Матриця ковариації.</param>
        /// <returns>Матриця власних векторів.</returns>
        private double[,] FindEigenVectors(double[,] covarianceMatrix)
        {
            var evd = new Accord.Math.Decompositions.EigenvalueDecomposition(covarianceMatrix);
            return evd.Eigenvectors;
        }

        /// <summary>
        /// Застосовує перетворення до центрованих пікселів, використовуючи власні вектори.
        /// </summary>
        /// <param name="pixels">Центровані пікселі у вигляді двовимірного масиву.</param>
        /// <param name="eigenVectors">Матриця власних векторів.</param>
        /// <returns>Результат перетворення у вигляді двовимірного масиву.</returns>
        /// <exception cref="InvalidOperationException">Викидається, якщо розміри матриць не відповідають.</exception>
        private double[,] ApplyTransformation(double[,] pixels, double[,] eigenVectors)
        {
            int pixelsRows = pixels.GetLength(0);
            int pixelsCols = pixels.GetLength(1);
            int eigenRows = eigenVectors.GetLength(0);
            int eigenCols = eigenVectors.GetLength(1);

            if (pixelsCols != eigenRows)
            {
                throw new InvalidOperationException("Невідповідність розмірів матриць для множення.");
            }

            double[,] result = new double[pixelsRows, eigenCols];

            for (int i = 0; i < pixelsRows; i++)
            {
                for (int j = 0; j < eigenCols; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < pixelsCols; k++)
                    {
                        sum += pixels[i, k] * eigenVectors[k, j];
                    }
                    result[i, j] = sum;
                }
            }

            return result;
        }

        /// <summary>
        /// Перетворює масив пікселів назад у зображення.
        /// </summary>
        /// <param name="pixels">Двовимірний масив пікселів.</param>
        /// <param name="width">Ширина зображення.</param>
        /// <param name="height">Висота зображення.</param>
        /// <returns>Зображення у форматі <see cref="Bitmap"/>.</returns>
        private Bitmap PixelsToImage(double[,] pixels, int width, int height, ColorChannel channel)
        {
            Bitmap result = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int value = Math.Min(255, Math.Max(0, (int)pixels[index, 0]));

                    Color newPixel;
                    switch (channel)
                    {
                        case ColorChannel.R:
                            newPixel = Color.FromArgb(value, 0, 0);
                            break;
                        case ColorChannel.G:
                            newPixel = Color.FromArgb(0, value, 0);
                            break;
                        case ColorChannel.B:
                            newPixel = Color.FromArgb(0, 0, value);
                            break;
                        default:
                            newPixel = Color.Black;
                            break;
                    }

                    result.SetPixel(x, y, newPixel);
                }
            }
            return result;
        }
    }
}
