using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GifCreator
{
    class PicEdit
    {
        List<Data> m_Data = new List<Data>();
        string m_path = "";
        public void OpenImgDir()
        {
            m_path = "";
            m_Data.Clear();
            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.Description = "选择图像目录";
            if (System.Windows.Forms.DialogResult.OK != fb.ShowDialog()) return;

            m_path = fb.SelectedPath;
            DirectoryInfo TheFolder = new DirectoryInfo(fb.SelectedPath);
            foreach (FileInfo NextFile in TheFolder.GetFiles("*.jpg"))
            {
                Data dat = new Data();
                dat.info = NextFile.Name;
                m_Data.Add(dat);
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
                showImg(0);
            });
        }
        public void showImg(int num)
        {
            if (num < 0 || num >= m_Data.Count) return;
            if (!Directory.Exists(m_path)) return;

            try
            {
                string imgPathName = m_path + "\\" + m_Data[num].info;
                MainWindow.obj.img.Source = new BitmapImage(new Uri(imgPathName, UriKind.RelativeOrAbsolute));
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
        }

        public bool GetSize(ref int width, ref int height)
        {
            if (m_Data.Count == 0) return false;
            string imgPathName = m_path + "\\" + m_Data[0].info;
            return MainWindow.GetSize(ref width, ref height, imgPathName);
        }
        public string GenGif()
        {
            if (!Directory.Exists(m_path)) return "";

            string picDir = m_path;
            picDir = picDir.Replace("\\", "/");
            string param = System.AppDomain.CurrentDomain.BaseDirectory + "GifOpt.py" + " 0 \"" + picDir + "\"";
            MainWindow.obj.RunPy(param);
            return m_path + MainWindow.pyCombName;
        }
    }
}
