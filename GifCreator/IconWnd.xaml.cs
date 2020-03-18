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
    public partial class IconWnd
    {
        int[] size = new int[] { 16, 32, 48, 64, 128 };
        public string resultSize = null;
        public IconWnd()
        {
            InitializeComponent();
            cb.ItemsSource = size;
            cb.SelectedIndex = 4;
        } 
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if(-1 != cb.SelectedIndex)
            {
                string si = size[cb.SelectedIndex].ToString();
                resultSize = si + "x" + si;
                this.DialogResult = true;
            }         
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
