using ImagePCA.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ImagePCA.Logic
{
    /// <summary>
    /// Клас для обробки методом головних компонент (PCA).
    /// </summary>
    public class PCAProcessor : IPCAProcessor
    {
        /// <summary>
        /// Центрує дані шляхом віднімання середнього значення.
        /// </summary>
        public double[,] CenterData(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            double[,] centeredData = new double[rows, cols];
            double[] mean = new double[cols];

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < rows; i++)
                {
                    mean[j] += data[i, j];
                }
                mean[j] /= rows;
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    centeredData[i, j] = data[i, j] - mean[j];
                }
            }

            return centeredData;
        }

        /// <summary>
        /// Обчислює коваріаційну матрицю для даних.
        /// </summary>
        public double[,] ComputeCovarianceMatrix(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            double[,] covarianceMatrix = new double[cols, cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < rows; k++)
                    {
                        sum += data[k, i] * data[k, j];
                    }
                    covarianceMatrix[i, j] = sum / (rows - 1);
                }
            }

            return covarianceMatrix;
        }

        /// <summary>
        /// Виконує декомпозицію власних значень для матриці.
        /// </summary>
        public (double[], double[,]) EigenDecomposition(double[,] matrix)
        {
            var mat = DenseMatrix.OfArray(matrix);
            var evd = mat.Evd();
            var eigenValues = evd.EigenValues.Real().ToArray();
            var eigenVectors = evd.EigenVectors.ToArray();
            return (eigenValues, eigenVectors);
        }

        /// <summary>
        /// Перетворює дані за допомогою власних векторів.
        /// </summary>
        public double[,] TransformData(double[,] data, double[,] eigenVectors)
        {
            int rows = data.GetLength(0);
            int cols = eigenVectors.GetLength(1);
            double[,] transformedData = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    for (int k = 0; k < data.GetLength(1); k++)
                    {
                        transformedData[i, j] += data[i, k] * eigenVectors[k, j];
                    }
                }
            }

            return transformedData;
        }
    }
}