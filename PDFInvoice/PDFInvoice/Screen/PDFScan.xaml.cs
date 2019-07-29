using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Spire.Pdf;

namespace PDFInvoice.Screen
{
    /// <summary>
    /// PDFScan.xaml 的交互逻辑
    /// </summary>
    public partial class PDFScan : UserControl
    {
        private string[] pdfFileName { get; set; }
        private string pdfPath { get; set; }
        private int currentPageNum { get; set; }
        private bool _isLoaded { get; set; } = false;
        private bool _isFileLoad { get; set; } = false;

        public PDFScan()
        {
            InitializeComponent();
            this.Loaded += PDFScan_Loaded;
            _isFileLoad = false;
        }

        private void PDFScan_Loaded(object sender, RoutedEventArgs e)
        {
            btnPrevious.Visibility = Visibility.Hidden;
            btnNext.Visibility = Visibility.Hidden;
            labelCount.Visibility = Visibility.Hidden;
            labelCurrent.Visibility = Visibility.Hidden;
            labelSplit.Visibility = Visibility.Hidden;
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            currentPageNum = 1;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "pdf File|*.pdf";
            openFileDialog.FileName = "选择文件夹.";

            openFileDialog.Multiselect = true; //允许同时选择多个文件

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
            {
                _isFileLoad = false;
                return;
            }
            else
            {
                pdfFileName = openFileDialog.FileNames;
                pdfPath = openFileDialog.FileName;
                if (pdfFileName.Length > 1)
                {
                    btnPrevious.Visibility = Visibility.Visible;
                    btnNext.Visibility = Visibility.Visible;
                    labelCount.Visibility = Visibility.Visible;
                    labelCurrent.Visibility = Visibility.Visible;
                    labelSplit.Visibility = Visibility.Visible;

                    labelCount.Content = pdfFileName.Length;
                }
                _isFileLoad = true;

                InitialPageNum();
                PreviewPDF();
            }
        }

        private void PreviewPDF()
        {
            try
            {
                labelFilePath.Content = pdfFileName[currentPageNum - 1];
                labelCurrent.Content = currentPageNum;
                moonPdfPanel.OpenFile(pdfFileName[currentPageNum - 1]);
                _isLoaded = true;
            }
            catch (Exception)
            {
                _isLoaded = false;
            }
        }

        private void InitialPageNum()
        {
            if (pdfFileName.Length == currentPageNum)
            {
                btnNext.IsEnabled = false;
            }
            else
            {
                btnNext.IsEnabled = true;
            }
            if (currentPageNum == 1)
            {
                btnPrevious.IsEnabled = false;
            }
            else
            {
                btnPrevious.IsEnabled = true;
            }
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ZoomIn();
            }
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ZoomOut();
            }
        }

        private void NormalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.Zoom(1.0);
            }
        }

        //整页
        private void FitToHeightButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ZoomToHeight();
            }
        }

        //双页
        private void FacingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ViewType = MoonPdfLib.ViewType.Facing;
            }
        }

        //单页
        private void SinglePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ViewType = MoonPdfLib.ViewType.SinglePage;
            }
        }

        //绘制状态
        private bool _started;

        //鼠标位置
        private System.Windows.Point _downPoint;

        //起始点点击
        private bool _firstClick = true;

        //X
        private System.Windows.Point x;

        //Y
        private System.Windows.Point y;

        private void MoonPdfPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_started && _isFileLoad)
            {
                var point = e.GetPosition(GridBody);

                var rect = new Rect(_downPoint, point);
                Rectangle.Margin = new Thickness(rect.Left, rect.Top - 30, 0, 0);
                Rectangle.Width = rect.Width;
                Rectangle.Height = rect.Height;
            }
        }

        private void MoonPdfPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isFileLoad)
            {
                btnZoned.Visibility = Visibility.Visible;
                Rectangle.Visibility = Visibility.Visible;
                if (moonPdfPanel.CurrentZoom != 1.00f)
                {
                    MessageBox.Show("请确保当前PDF显示比例为100%");
                    return;
                }

                _downPoint = e.GetPosition(GridBody);
                if (_firstClick)
                {
                    _started = true;
                    _firstClick = false;
                    x = _downPoint;
                }
                else
                {
                    _started = false;
                    _firstClick = true;
                    y = _downPoint;
                    getPDFMsg();
                }
            }
        }

        /// <summary>
        /// 因为鼠标在弹起的时候有几率会在矩形中弹起，所以也需要在Rectangle中将buttonup事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isFileLoad)
            {
                _downPoint = e.GetPosition(GridBody);
                btnZoned.Visibility = Visibility.Visible;
                Rectangle.Visibility = Visibility.Visible;
                if (_firstClick)
                {
                    _started = true;
                    _firstClick = false;
                    x = _downPoint;
                }
                else
                {
                    _started = false;
                    _firstClick = true;
                    y = _downPoint;
                    getPDFMsg();
                }
            }
        }

        private void getPDFMsg()
        {
            PdfDocument pdf = new PdfDocument();

            pdf.LoadFromFile(pdfFileName[currentPageNum - 1]);

            PdfPageBase page = pdf.Pages[0];

            //从第一页的指定矩形区域内提取文本
            string text = page.ExtractText(new RectangleF((int)(x.X - 70), (int)(x.Y - 30), (int)(y.X - 80), (int)(y.Y - 135)));
            //string text = page.ExtractText(new RectangleF(50, 50, 100, 100));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(text);
            File.WriteAllText("Extract.txt", sb.ToString().Replace("Evaluation Warning : The document was created with Spire.PDF for .NET.", ""));
            Process.Start("Extract.txt");
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            --currentPageNum;
            InitialPageNum();
            PreviewPDF();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            ++currentPageNum;
            InitialPageNum();
            PreviewPDF();
        }

        private void BtnZoned_Click(object sender, RoutedEventArgs e)
        {
            btnZoned.Visibility = Visibility.Hidden;
            Rectangle.Visibility = Visibility.Hidden;
        }
    }
}