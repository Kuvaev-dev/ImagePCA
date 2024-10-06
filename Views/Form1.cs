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

        // Ін'єкція стратегії обробки зображень через інтерфейс
        public Form1(IImageProcessor imageProcessor)
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            _imageProcessor = imageProcessor;
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                pictureBoxOriginal.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBoxOriginal.Image = originalImage;
            }
        }

        private void btnProcessImage_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Сначала загрузите изображение.");
                return;
            }

            ColorChannel selectedChannel = GetSelectedChannel();
            processedImage = _imageProcessor.ProcessImage(originalImage, selectedChannel);
            pictureBoxProcessed.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxProcessed.Image = processedImage;
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (processedImage == null)
            {
                MessageBox.Show("Изображение еще не обработано.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                processedImage.Save(saveFileDialog.FileName);
            }
        }

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

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxChannels.Items.AddRange(new string[] { "R", "G", "B" });
            comboBoxChannels.SelectedIndex = 0;
        }
    }
}