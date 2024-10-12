using System;

namespace ImagePCA.Views
{
    public class TextViewer
    {
        // Метод для зміни кольору тексту у консолі
        public static void ChangeColor(string text, ConsoleColor color)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
