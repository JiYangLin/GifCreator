

using System.IO;

namespace GifCreator
{
    class GifResize
    {
        public void Resize(string gifPathName,int width, int height)
        {
            if (!File.Exists(gifPathName)) return;

            gifPathName = gifPathName.Replace("\\", "/");
            string param = System.AppDomain.CurrentDomain.BaseDirectory + "GifOpt.py";
            param = param + " " + "1";
            param = param + " " + "\"" + gifPathName + "\"";
            param = param + " " + width.ToString();
            param = param + " " + height.ToString();
            MainWindow.obj.RunPy(param);
        }
    }
}
