using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GifCreator
{
    /// <summary>
    /// OutputInfoWnd.xaml 的交互逻辑
    /// </summary>
    public partial class OutputInfoWnd : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int w = 0;
        public int WITH
        {
            get { return w; }
            set
            {
                w = value;
                OnPropertyChanged("WITH");
            }
        }
        public int h = 0;
        public int HEIGHT
        {
            get { return h; }
            set
            {
                h = value;
                OnPropertyChanged("HEIGHT");
            }
        }


        public OutputInfoWnd(int width, int height)
        {
            InitializeComponent();
            this.DataContext = this;
            w = width;
            h = height;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
