using ImagePCA.Interfaces;
using ImagePCA.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImagePCA.Logic
{
    /// <summary>
    /// Реалізація стратегії обробки зображень за допомогою PCA (метод головних компонент).
    /// </summary>
    public class PCAImageProcessor : IImageProcessor
    {
        /// <summary>
        /// Основний метод обробки зображення за допомогою PCA для вибраного каналу кольору.
        /// Виводить проміжні етапи обробки у консоль.
        /// </summary>
        /// <param name="originalImage">Оригінальне зображення для обробки.</param>
        /// <param name="channel">Канал кольору для обробки (R, G або B).</param>
        /// <returns>Оброблене зображення після застосування PCA.</returns>
        public Bitmap ProcessImage(Bitmap originalImage, ColorChannel channel)
        {
            Console.Clear();

            TextViewer.ChangeColor("Початок обробки зображення...\n", ConsoleColor.Red);

            var pixels = ImageToPixels(originalImage, channel);
            TextViewer.ChangeColor($"Перетворено на масив пікселів. Приклад перших 5 значень: \n{string.Join(", ", GetFirstElements(pixels, 5))}\n", ConsoleColor.Green);

            var meanVector = CalculateMean(pixels);
            TextViewer.ChangeColor($"Середнє значення: \n{string.Join(", ", meanVector.Select(m => m.ToString("F2")))}\n", ConsoleColor.Green);

            var centeredPixels = CenterData(pixels, meanVector);
            TextViewer.ChangeColor($"Центровані дані. Приклад перших 5 значень: \n{string.Join(", ", GetFirstElements(centeredPixels, 5))}\n", ConsoleColor.Green);

            var covarianceMatrix = CalculateCovarianceMatrix(centeredPixels);
            TextViewer.ChangeColor($"Матриця коваріації (перших кілька елементів): \n{GetMatrixSummary(covarianceMatrix)}\n", ConsoleColor.Green);

            var eigenVectors = FindEigenVectors(covarianceMatrix);
            TextViewer.ChangeColor($"Власні вектори (перших кілька елементів): \n{GetMatrixSummary(eigenVectors)}\n", ConsoleColor.Green);

            var transformedPixels = ApplyTransformation(centeredPixels, eigenVectors);
            TextViewer.ChangeColor($"Перетворені пікселі. Приклад перших 5 значень: \n{string.Join(", ", GetFirstElements(transformedPixels, 5))}\n", ConsoleColor.Green);

            TextViewer.ChangeColor("Обробку завершено.\n", ConsoleColor.Cyan);

            return PixelsToImage(transformedPixels, originalImage.Width, originalImage.Height, channel);
        }

        /// <summary>
        /// Повертає перші N елементів із матриці пікселів у форматі рядка.
        /// </summary>
        /// <param name="matrix">Двовимірна матриця пікселів.</param>
        /// <param name="count">Кількість елементів для відображення.</param>
        /// <returns>Рядок із перших N елементів.</returns>
        private string GetFirstElements(double[,] matrix, int count)
        {
            return string.Join(", ", Enumerable.Range(0, Math.Min(matrix.GetLength(0), count))
                                               .Select(i => matrix[i, 0].ToString("F2")));
        }

        /// <summary>
        /// Формує підсумок матриці, обмежений розміром 3x3, для виведення в консоль.
        /// </summary>
        /// <param name="matrix">Двовимірна матриця.</param>
        /// <returns>Рядок із підсумком матриці.</returns>
        private string GetMatrixSummary(double[,] matrix)
        {
            // Обмежуємо розмір матриці до 3x3
            int rows = Math.Min(matrix.GetLength(0), 3);
            int cols = Math.Min(matrix.GetLength(1), 3);
            var summary = new List<string>();

            for (int i = 0; i < rows; i++)
            {
                var row = string.Join(", ", Enumerable.Range(0, cols)
                                                      .Select(j => matrix[i, j].ToString("F2")));
                summary.Add($"[{row}]");
            }
            return string.Join("; ", summary);
        }

        /// <summary>
        /// Перетворює зображення у двовимірний масив пікселів для вибраного каналу кольору.
        /// </summary>
        /// <param name="image">Зображення, яке потрібно перетворити.</param>
        /// <param name="channel">Канал кольору (R, G або B).</param>
        /// <returns>Двовимірний масив пікселів.</returns>
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
        /// Застосовує перетворення PCA до центрованих пікселів на основі власних векторів.
        /// </summary>
        /// <param name="centeredPixels">Центровані пікселі у вигляді двовимірного масиву.</param>
        /// <param name="eigenVectors">Матриця власних векторів.</param>
        /// <returns>Перетворені пікселі після застосування PCA.</returns>
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
        /// Перетворює оброблені пікселі назад у зображення.
        /// </summary>
        /// <param name="pixels">Перетворені пікселі у вигляді двовимірного масиву.</param>
        /// <param name="width">Ширина зображення.</param>
        /// <param name="height">Висота зображення.</param>
        /// <param name="channel">Канал кольору для обробки (R, G або B).</param>
        /// <returns>Зображення після перетворення.</returns>
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
