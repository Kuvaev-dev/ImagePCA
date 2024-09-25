using ImagePCA.Interfaces;
using ImagePCA.Logic;
using ImagePCA.Views;
using System;
using System.Windows.Forms;

namespace ImagePCA
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IImageProcessor imageProcessor = new PCAImageProcessor();
            Application.Run(new Form1(imageProcessor));
        }
    }
}
