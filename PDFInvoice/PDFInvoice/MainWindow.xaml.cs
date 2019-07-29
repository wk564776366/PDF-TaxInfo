using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PDFInvoice.Screen;

namespace PDFInvoice
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.MouseLeftButtonUp += MainWindow_MouseLeftButtonUp;
        }

        private void MainWindow_changeTabEvent(int index)
        {
            switch (index)
            {
                case 1:
                    TB1.IsSelected = true;
                    break;

                case 2:
                    TB2.IsSelected = true;
                    break;

                case 3:
                    TB3.IsSelected = true;
                    break;
            }
        }

        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}