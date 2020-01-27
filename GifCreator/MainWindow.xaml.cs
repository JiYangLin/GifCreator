using Comm;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GifCreator
{

    public class Data
    {
        public int num { get; set; }
        public string info { get; set; }

        public int frame;
    }

    public delegate bool ProcFun();

    public enum ProcType
    {
        ProcType_video,
        ProcType_pic
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public static string pyCombName = "--Gif.gif";
        public static bool GetSize(ref int w, ref int h, string imgPathName)
        {
            using (FileStream fs = new FileStream(imgPathName, FileMode.Open, FileAccess.Read))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                w = image.Width;
                h = image.Height;
            }

            OutputInfoWnd wnd = new OutputInfoWnd(w, h);
            if (wnd.ShowDialog() != true) return false;

            w = wnd.w;
            h = wnd.h;

            return true;
        }


        #region threadctl
        bool m_Runing = false;
        bool isRunning()
        {
            if (m_Runing)
            {
                OptWaitWnd wnd = new OptWaitWnd("", "正在运行,请稍后操作");
                wnd.ShowDialog();
                return true;
            }
            return false;
        }
        delegate void thrRun();
        void Run(thrRun fun)
        {
            if (isRunning()) return;
            m_Runing = true;
            Thread thread = new Thread(() => {
                try
                {
                    fun();
                    m_Runing = false;
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() => {
                        OptWaitWnd wnd = new OptWaitWnd("", ex.ToString());
                        wnd.ShowDialog();
                    });
                    m_Runing = false;
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
        #endregion


        #region pythonRun
        public void RunPy(string param)
        {
            string python = System.AppDomain.CurrentDomain.BaseDirectory + "python381\\python.exe";
            if (!File.Exists(python))
            {
                MessageBox.Show("python解释器不存在");
                return;
            }

            Process p = new Process();
            p.OutputDataReceived += OutputDataReceived;
            p.StartInfo = new ProcessStartInfo(python, param)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            };
            p.Start();
            p.BeginOutputReadLine();
        }
        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string data = e.Data;
            if (null == data) return;
            Dispatcher.Invoke(() => {
                status.Content = data;
            });
        }
        #endregion


        public static MainWindow obj = null;
        VideoEdit m_VideoEdit = new VideoEdit();
        PicEdit m_PicEdit = new PicEdit();
     

        public MainWindow()
        {
            InitializeComponent();
            obj = this;
        }
        void ProcOpt(ProcFun funA, ProcFun funB)
        {
            if (!funA()) return;
            Run(() => {
                OptWaitWnd wnd = null;
                this.Dispatcher.Invoke(() => {
                    wnd = new OptWaitWnd("", "正在处理请稍后");
                    wnd.Show();
                });
                bool ret = funB();
                this.Dispatcher.Invoke(() => {
                    wnd.Close();
                    if (ret) return;
                    wnd = new OptWaitWnd("", "生成失败");
                    wnd.ShowDialog();
                });
            });
        }

        ProcType m_ProcType = ProcType.ProcType_video;
        private void SourceList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isRunning()) return;
            if (SourceList.SelectedIndex < 0) return;

            if(m_ProcType == ProcType.ProcType_video)
            {
                m_VideoEdit.showFrame(SourceList.SelectedIndex);
            }
            else
            {
                m_PicEdit.showImg(SourceList.SelectedIndex);
            }
        }



        private void OpenVideo_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            m_ProcType = ProcType.ProcType_video;
            m_VideoEdit.OpenVideo();      
        }
        private void AutoPlay_Click(object sender, RoutedEventArgs e)
        {
            Run(()=> {
                m_VideoEdit.AutoPlay();
            });
        }
        private void StopPlay_Click(object sender, RoutedEventArgs e)
        {
            m_VideoEdit.StopPlay();
        }
        private void DelFrame_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            m_VideoEdit.DelSel();
        }


        private void VideoToGif_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            int width = 0;
            int height = 0;
            ProcOpt(
                () => {
                    return m_VideoEdit.GetSize(ref width, ref height);
                },

                () => {
                string dir = m_VideoEdit.SplitGif(width, height);
                m_VideoEdit.SaveGif(dir);
                Thread.Sleep(2000);
                string dst = dir + pyCombName;
                if (!File.Exists(dst)) return false;
                Process.Start(dst);
                return true;
            });
        }
        private void SplitToImg_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            int width = 0;
            int height = 0;
            ProcOpt(
            () => {
                return m_VideoEdit.GetSize(ref width, ref height);
            }, 

            () => {
                string dir = m_VideoEdit.SplitGif(width,height);
                if (!Directory.Exists(dir)) return false;
                Process.Start(dir);
                return true;
            });
        }






        private void ImportImg_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            m_PicEdit.OpenImgDir();
            m_ProcType = ProcType.ProcType_pic;
        }
        private void CombinGif_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            ProcOpt(
                () => {
                    return true;
                },

                () => {
                    string dst = m_PicEdit.GenGif();
                    Thread.Sleep(2000);
                    if (!File.Exists(dst)) return false;
                    Process.Start(dst);
                    return true;
                });
        }
        private void ReSizeGif_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning()) return;
            var of = new Microsoft.Win32.OpenFileDialog() { Filter = "GIF|*.gif||" };
            if (true != of.ShowDialog()) return;
            string gifPathName = of.FileName;

            GifResize _GifResize = new GifResize();
            int width = 0;
            int height = 0;
            ProcOpt(
                () => {
                    return GetSize(ref width, ref height, gifPathName);
                },

                ()=> {
                    _GifResize.Resize(gifPathName,width, height);
                    Thread.Sleep(2000);
                    string dst = gifPathName + "--resize.gif";
                    if (!File.Exists(dst)) return false;
                    Process.Start(dst);
                    return true;
                });
        }
    }
}
