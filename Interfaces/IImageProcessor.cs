using System.Drawing;

namespace ImagePCA.Interfaces
{
    /// <summary>
    /// Інтерфейс для обробки зображень.
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Отримує дані каналів зображення.
        /// </summary>
        /// <param name="image">Зображення, з якого потрібно отримати дані.</param>
        /// <returns>Двовимірний масив з даними каналів.</returns>
        double[,] GetChannelData(Bitmap image);

        /// <summary>
        /// Створює зображення з матриці даних.
        /// </summary>
        /// <param name="matrix">Матриця даних.</param>
        /// <param name="originalImage">Оригінальне зображення.</param>
        /// <param name="channelIndex">Індекс каналу для створення зображення.</param>
        /// <returns>Зображення, створене з матриці.</returns>
        Bitmap CreateImageFromMatrix(double[,] matrix, Bitmap originalImage, int channelIndex);

        /// <summary>
        /// Створює зображення з перетворених даних.
        /// </summary>
        /// <param name="transformedData">Перетворені дані.</param>
        /// <param name="originalImage">Оригінальне зображення.</param>
        /// <returns>Зображення, створене з перетворених даних.</returns>
        Bitmap CreateImageFromTransformedData(double[,] transformedData, Bitmap originalImage);
    }
}