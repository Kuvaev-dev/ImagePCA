using ImagePCA.Interfaces;
using ImagePCA.Logic;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImagePCA.Views
{
    /// <summary>
    /// Головна форма програми.
    /// </summary>
    public partial class Form1 : Form
    {
        private Bitmap originalImage;
        private Bitmap transformedImage;
        private readonly IImageProcessor imageProcessor;
        private readonly IPCAProcessor pcaProcessor;

        /// <summary>
        /// Конструктор форми.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            comboBoxChannels.Items.AddRange(new string[] { "Red", "Green", "Blue", "All" });
            comboBoxChannels.SelectedIndex = 0;

            imageProcessor = new ImageProcessor();
            pcaProcessor = new PCAProcessor();

            // Встановлення режиму адаптації зображень у PictureBox
            pictureBoxOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxProcessed.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// Завантажує зображення за допомогою OpenFileDialog.
        /// </summary>
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                pictureBoxOriginal.Image = originalImage;
            }
        }

        /// <summary>
        /// Застосовує метод PCA до завантаженого зображення.
        /// </summary>
        private void btnApplyPCA_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Завантажте зображення, будь-ласка.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double[,] channelData = imageProcessor.GetChannelData(originalImage);
            TextViewer.ChangeColor("Дані каналу:", ConsoleColor.Cyan);
            PrintMatrix(channelData);

            double[,] centeredData = pcaProcessor.CenterData(channelData);
            TextViewer.ChangeColor("Центровані дані:", ConsoleColor.Cyan);
            PrintMatrix(centeredData);

            double[,] covarianceMatrix = pcaProcessor.ComputeCovarianceMatrix(centeredData);
            TextViewer.ChangeColor("Матриця ковариації:", ConsoleColor.Cyan);
            PrintMatrix(covarianceMatrix);

            var (eigenValues, eigenVectors) = pcaProcessor.EigenDecomposition(covarianceMatrix);
            TextViewer.ChangeColor("Знаення власних векторів:", ConsoleColor.Cyan);
            PrintArray(eigenValues);
            TextViewer.ChangeColor("Власні вектори:", ConsoleColor.Cyan);
            PrintMatrix(eigenVectors);

            double[,] transformedData = pcaProcessor.TransformData(centeredData, eigenVectors);
            TextViewer.ChangeColor("Трансформовані дані:", ConsoleColor.Cyan);
            PrintMatrix(transformedData);

            if (comboBoxChannels.SelectedItem.ToString() == "All")
            {
                transformedImage = imageProcessor.CreateImageFromTransformedData(transformedData, originalImage);
            }
            else
            {
                transformedImage = imageProcessor.CreateImageFromMatrix(transformedData, originalImage, comboBoxChannels.SelectedIndex);
            }
            pictureBoxProcessed.Image = transformedImage;
        }

        /// <summary>
        /// Зберігає перетворене зображення.
        /// </summary>
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (transformedImage == null)
            {
                MessageBox.Show("Немає трансформованого зображення для збереження.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            saveFileDialog.Title = "Зберегти трансформоване зображення";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                transformedImage.Save(saveFileDialog.FileName);
                MessageBox.Show("Зображення успішно збережено.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Виводить матрицю у консоль.
        /// </summary>
        private void PrintMatrix(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int count = 0;
            for (int i = 0; i < rows && count < 9; i++)
            {
                for (int j = 0; j < cols && count < 9; j++)
                {
                    Console.Write($"{matrix[i, j]} ");
                    count++;
                }
                Console.WriteLine();
            }
            Console.WriteLine("...");
        }

        /// <summary>
        /// Виводить масив у консоль.
        /// </summary>
        private void PrintArray(double[] array)
        {
            for (int i = 0; i < array.Length && i < 5; i++)
            {
                Console.Write($"{array[i]} ");
            }
            Console.WriteLine("...");
        }

        /// <summary>
        /// Обробник події завантаження форми.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}