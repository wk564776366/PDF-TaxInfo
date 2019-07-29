using System;
using System.Collections.Generic;
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

namespace PDFInvoice
{
    /// <summary>
    /// PreviewPdf.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewPdf : Window
    {
        private string path;

        public PreviewPdf(string path)
        {
            InitializeComponent();
            this.Loaded += PreviewPdf_Loaded;
            this.path = path;
        }

        private void PreviewPdf_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                moonPdfPanel.OpenFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomOut();
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ZoomIn();
        }
    }
}