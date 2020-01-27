using OpenCvSharp;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using Size = OpenCvSharp.Size;

namespace GifCreator
{
    class VideoEdit
    {
        VideoCapture m_capture = null;
        List<Data> m_Data = new List<Data>();
        string m_pathName = "";

        public void DelSel()
        {
            List<Data> itemList = new List<Data>();
            foreach (Data item in MainWindow.obj.SourceList.SelectedItems)
            {
                itemList.Add(item);
            }
            foreach (Data item in itemList)
            {
                m_Data.Remove(item);
            }
            FlushDg();
        }
        void FlushDg()
        {
            for (int i = 0; i < m_Data.Count; ++i) m_Data[i].num = i;
            MainWindow.obj.Dispatcher.Invoke(() => {
                MainWindow.obj.SourceList.DataContext = null;
                MainWindow.obj.SourceList.DataContext = m_Data;
                MainWindow.obj.SourceList.SelectedIndex = 0;
                showFrame(0);
            });
        }
        public void OpenVideo()
        {
            var of = new Microsoft.Win32.OpenFileDialog() { Filter = "视频文件|*.*||" };
            if (true != of.ShowDialog()) return;

            m_capture = null;
            m_Data.Clear();
            m_pathName = of.FileName;
            m_capture = new VideoCapture(of.FileName);
            for (int i = 0; i < m_capture.FrameCount; ++i)
            {
                Data dat = new Data();
                dat.num = i + 1;
                dat.info = "第" + (i + 1).ToString() + "帧";
                dat.frame = i;
                m_Data.Add(dat);
            }

            FlushDg();
        }
        public void showFrame(int frame)
        {
            if (null == m_capture) return;
            m_capture.PosFrames = frame;
            Mat image = new Mat();
            m_capture.Read(image);
            if (null == image) return;

            Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                MainWindow.obj.img.Source = result;
            }
            image.Release();
        }

        bool autoplaying = false;
        public void AutoPlay()
        {
            if (null == m_capture) return;
            autoplaying = true;
            foreach(Data item in m_Data)
            {
                if (!autoplaying) break;
                MainWindow.obj.Dispatcher.Invoke(()=> {
                    MainWindow.obj.SourceList.SelectedIndex = item.num;
                    MainWindow.obj.SourceList.ScrollIntoView(MainWindow.obj.SourceList.SelectedItem);
                    showFrame(item.frame);
                });
                Thread.Sleep(200);
            }
            autoplaying = false;
        }
        public void StopPlay()
        {
            autoplaying = false;
        }

        public bool GetSize(ref int w,ref int h)
        {
            if (m_capture == null) return false;

            w = m_capture.FrameWidth;
            h = m_capture.FrameHeight;

            OutputInfoWnd wnd = new OutputInfoWnd(w, h);
            if (wnd.ShowDialog() != true) return false;

            w = wnd.w;
            h = wnd.h;

            return true;
        }




        public string SplitGif(int width, int height)
        {
            bool useDefaultSize = 
            width == m_capture.FrameWidth &&
            height == m_capture.FrameHeight;

            string dir = m_pathName + "--splitImg";
            Directory.CreateDirectory(dir);
            foreach (Data item in m_Data)
            {
                m_capture.PosFrames = item.frame;
                MainWindow.obj.Dispatcher.Invoke(() => {
                    MainWindow.obj.SourceList.SelectedIndex = item.frame;
                    MainWindow.obj.SourceList.ScrollIntoView(MainWindow.obj.SourceList.SelectedItem);
                });

                Mat image = new Mat();
                m_capture.Read(image);
                if (null == image) continue;
                Size si = new Size();
                si.Width = width;
                si.Height = height;
                string pathName = dir + "\\" + (item.frame + 1000000).ToString() + ".jpg";
                if (!useDefaultSize)
                {
                    Mat im  = image.Resize(si);
                    im.SaveImage(pathName);
                    im.Release();
                }
                else
                {
                    image.SaveImage(pathName);
                    image.Release();
                }
            }
            return dir;
        }
        public void SaveGif(string picDir)
        {
           if (!Directory.Exists(picDir)) return;
           picDir = picDir.Replace("\\", "/");
           string param = System.AppDomain.CurrentDomain.BaseDirectory + "GifOpt.py" + " 0 \"" + picDir + "\"";
           MainWindow.obj.RunPy(param);
        }
    }
}
