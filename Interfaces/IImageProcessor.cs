using System.Drawing;

namespace ImagePCA.Interfaces
{
    /// <summary>
    /// Перерахування каналів кольору (Червоний, Зелений, Синій).
    /// </summary>
    public enum ColorChannel
    {
        R,
        G,
        B
    }

    /// <summary>
    /// Інтерфейс для процесора зображень, що визначає метод обробки зображень.
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Метод для обробки зображення відповідно до вибраного каналу кольору.
        /// </summary>
        /// <param name="image">Вхідне зображення для обробки.</param>
        /// <param name="channel">Канал кольору для обробки (R, G або B).</param>
        /// <returns>Оброблене зображення.</returns>
        Bitmap ProcessImage(Bitmap image, ColorChannel channel);
    }
}