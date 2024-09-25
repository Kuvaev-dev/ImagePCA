using System.Drawing;

namespace ImagePCA.Interfaces
{
    /// <summary>
    /// Інтерфейс для стратегії обробки зображень.
    /// Цей інтерфейс визначає метод <see cref="ProcessImage"/> для обробки зображення.
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Обробляє задане зображення.
        /// </summary>
        /// <param name="image">Зображення, яке потрібно обробити.</param>
        /// <returns>Оброблене зображення у форматі <see cref="Bitmap"/>.</returns>
        Bitmap ProcessImage(Bitmap image);
    }
}