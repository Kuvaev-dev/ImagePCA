using ImagePCA.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImagePCA.Views
{
    public partial class Form1 : Form
    {
        private Bitmap originalImage;
        private Bitmap processedImage;
        private readonly IImageProcessor _imageProcessor;

        /// <summary>
        /// Конструктор форми. Виконується ін'єкція стратегії обробки зображень через інтерфейс.
        /// </summary>
        /// <param name="imageProcessor">Інтерфейс для обробки зображень.</param>
        public Form1(IImageProcessor imageProcessor)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle; // Забороняє змінювати розмір вікна
            _imageProcessor = imageProcessor;
        }

        /// <summary>
        /// Обробляє подію натискання на кнопку "Завантажити зображення".
        /// Відкриває діалог вибору файлу і завантажує вибране зображення.
        /// </summary>
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" // Фільтр для зображень
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                pictureBoxOriginal.SizeMode = PictureBoxSizeMode.StretchImage; // Розтягує зображення для кращого перегляду
                pictureBoxOriginal.Image = originalImage;
            }
        }

        /// <summary>
        /// Обробляє подію натискання на кнопку "Обробити зображення".
        /// Використовує обраний канал кольору для обробки зображення.
        /// </summary>
        private void btnProcessImage_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Сначала загрузите изображение."); // Повідомлення, якщо зображення не завантажено
                return;
            }

            ColorChannel selectedChannel = GetSelectedChannel(); // Отримує вибраний канал
            processedImage = _imageProcessor.ProcessImage(originalImage, selectedChannel); // Виклик методу обробки зображення
            pictureBoxProcessed.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxProcessed.Image = processedImage;
        }

        /// <summary>
        /// Обробляє подію натискання на кнопку "Зберегти зображення".
        /// Відкриває діалог збереження файлу і дозволяє зберегти оброблене зображення.
        /// </summary>
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (processedImage == null)
            {
                MessageBox.Show("Изображение еще не обработано."); // Повідомлення, якщо зображення не оброблено
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp" // Фільтр для вибору типу збереженого файлу
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                processedImage.Save(saveFileDialog.FileName); // Збереження обробленого зображення
            }
        }

        /// <summary>
        /// Отримує вибраний користувачем канал кольору з ComboBox.
        /// </summary>
        /// <returns>Вибраний канал кольору (R, G або B).</returns>
        private ColorChannel GetSelectedChannel()
        {
            switch (comboBoxChannels.SelectedItem.ToString())
            {
                case "R": return ColorChannel.R;
                case "G": return ColorChannel.G;
                case "B": return ColorChannel.B;
                default: return ColorChannel.R;
            }
        }

        /// <summary>
        /// Ініціалізація форми під час завантаження. Заповнює ComboBox доступними каналами кольору.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxChannels.Items.AddRange(new string[] { "R", "G", "B" });
            comboBoxChannels.SelectedIndex = 0; // Встановлює R як канал за замовчуванням
        }
    }
}