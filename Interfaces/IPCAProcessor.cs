namespace ImagePCA.Interfaces
{
    /// <summary>
    /// Інтерфейс для обробки методом головних компонент (PCA).
    /// </summary>
    public interface IPCAProcessor
    {
        /// <summary>
        /// Центрує дані шляхом віднімання середнього значення.
        /// </summary>
        /// <param name="data">Дані для центрування.</param>
        /// <returns>Центровані дані.</returns>
        double[,] CenterData(double[,] data);

        /// <summary>
        /// Обчислює коваріаційну матрицю для даних.
        /// </summary>
        /// <param name="data">Дані для обчислення коваріаційної матриці.</param>
        /// <returns>Коваріаційна матриця.</returns>
        double[,] ComputeCovarianceMatrix(double[,] data);

        /// <summary>
        /// Виконує декомпозицію власних значень для матриці.
        /// </summary>
        /// <param name="matrix">Матриця для декомпозиції.</param>
        /// <returns>Кортеж з власними значеннями та власними векторами.</returns>
        (double[], double[,]) EigenDecomposition(double[,] matrix);

        /// <summary>
        /// Перетворює дані за допомогою власних векторів.
        /// </summary>
        /// <param name="data">Дані для перетворення.</param>
        /// <param name="eigenVectors">Власні вектори для перетворення.</param>
        /// <returns>Перетворені дані.</returns>
        double[,] TransformData(double[,] data, double[,] eigenVectors);
    }
}