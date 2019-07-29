using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using GetPdfMessage;
using Microsoft.Win32;
using Spire.Pdf;

namespace PDFInvoice.Screen
{
    /// <summary>
    /// PDFManager.xaml 的交互逻辑
    /// </summary>
    public partial class PDFManager : UserControl
    {
        //PDF组路径
        private string[] pdfFileName { get; set; }

        private string pdfPath { get; set; }
        private string CompamyCount { get; set; } = string.Empty;

        //信息分组保存
        private Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

        #region button name define

        private string btnModify = "btnModify";
        private string btnView = "btnView";
        private string btnSave = "btnSave";
        private string btnCancel = "btnCancel";
        private string labelPath = "labelPath";

        #endregion button name define

        //随鼠标移动的水平线
        private Line line = new Line();

        //存储Label, btnModify, btnFind, btnSave
        private Dictionary<int, List<object>> dicControl = new Dictionary<int, List<object>>();

        public PDFManager()
        {
            InitializeComponent();
            btnInput.Click += BtnInput_Click;
        }

        private async void BtnInput_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "pdf File|*.pdf";
            openFileDialog.FileName = "选择文件夹.";

            openFileDialog.Multiselect = true; //允许同时选择多个文件

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }
            else
            {
                pdfFileName = openFileDialog.FileNames;
                pdfPath = openFileDialog.FileName;
                processBar.Value = 0;
            }
            await Task.Run(() => { assortSource(); });
        }

        private void assortSource()
        {
            try
            {
                PdfDocument document = new PdfDocument();
                CompamyCount = string.Empty;
                dic.Clear();
                for (int i = 0; i < pdfFileName.Length; i++)
                {
                    document.LoadFromFile(pdfFileName[i]);

                    StringBuilder content = new StringBuilder();
                    content.Append(document.Pages[0].ExtractText());

                    string pdfSource = content.ToString().Trim();
                    GetPdfMsg getMsg = new GetPdfMsg();

                    var companyName = getMsg.GetInf(pdfSource, InvoiceParam.keywordType.CompanyName);

                    Statistics(companyName, pdfFileName[i]);
                    Dispatcher.Invoke(() => { processBar.Value += 100 / (double)pdfFileName.Length / 2; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            InitialControl();
        }

        private void Statistics(string str, string filePath)
        {
            if (dic.ContainsKey(str))
            {
                dic[str].Add(filePath);
            }
            else
            {
                List<string> list = new List<string>();
                list.Add(filePath);
                dic.Add(str, list);
            }
        }

        private void InitialControl()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    dicControl.Clear();
                    gridControl.Children.Clear();
                    int index = 0;
                    foreach (KeyValuePair<string, List<string>> dicItem in dic)
                    {
                        Label label = new Label();
                        label.Name = "label" + index;
                        label.Height = 30;
                        label.Width = double.NaN;
                        label.HorizontalAlignment = HorizontalAlignment.Left;
                        label.VerticalAlignment = VerticalAlignment.Top;
                        label.Margin = new Thickness(30, index * 30 + 10, 0, 0);
                        label.Content = dicItem.Key + "：";
                        label.Foreground = new SolidColorBrush(Colors.White);
                        label.Background = new SolidColorBrush(Colors.Transparent);
                        gridControl.Children.Add(label);
                        index++;
                        var indexMark = index;
                        for (int i = index; i < indexMark + dicItem.Value.Count; i++, index++)
                        {
                            List<object> controlCollection = new List<object>();
                            //动态生成label
                            label = new Label();
                            label.Name = labelPath + i;
                            label.Height = 30;
                            label.Width = double.NaN;
                            label.HorizontalAlignment = HorizontalAlignment.Left;
                            label.VerticalAlignment = VerticalAlignment.Top;
                            label.Margin = new Thickness(60, i * 30 + 10, 0, 0);
                            label.Content = dicItem.Value[i - indexMark];
                            label.Foreground = new SolidColorBrush(Colors.White);
                            label.Background = new SolidColorBrush(Colors.Transparent);
                            gridControl.Children.Add(label);
                            controlCollection.Add(label);

                            //动态生成buttn1(修改)
                            Button btn = new Button();
                            btn.Name = btnModify + i;
                            btn.Height = 20;
                            btn.Width = 45;
                            btn.HorizontalAlignment = HorizontalAlignment.Right;
                            btn.VerticalAlignment = VerticalAlignment.Top;
                            btn.Margin = new Thickness(0, i * 30 + 11, 180, 0);
                            btn.Content = "移动";
                            btn.Foreground = new SolidColorBrush(Colors.Black);
                            gridControl.Children.Add(btn);
                            btn.Click += btnModify_Click;
                            controlCollection.Add(btn);

                            //动态生成buttn1(查看)
                            btn = new Button();
                            btn.Name = btnView + i;
                            btn.Height = 20;
                            btn.Width = 45;
                            btn.HorizontalAlignment = HorizontalAlignment.Right;
                            btn.VerticalAlignment = VerticalAlignment.Top;
                            btn.Margin = new Thickness(0, i * 30 + 11, 130, 0);
                            btn.Content = "查看";
                            btn.Foreground = new SolidColorBrush(Colors.Black);
                            gridControl.Children.Add(btn);
                            btn.Click += btnView_Click;
                            controlCollection.Add(btn);

                            //动态生成buttn1(保存)
                            btn = new Button();
                            btn.Name = btnSave + i;
                            btn.Height = 20;
                            btn.Width = 45;
                            btn.HorizontalAlignment = HorizontalAlignment.Right;
                            btn.VerticalAlignment = VerticalAlignment.Top;
                            btn.Margin = new Thickness(0, i * 30 + 11, 80, 0);
                            btn.Content = "保存";
                            btn.Foreground = new SolidColorBrush(Colors.Black);
                            gridControl.Children.Add(btn);
                            btn.Click += btnSave_Click;
                            btn.Visibility = Visibility.Hidden;
                            Dispatcher.Invoke(() => { processBar.Value += 100 / (double)pdfFileName.Length / 2; });
                            controlCollection.Add(btn);

                            //动态生成buttn1(取消)
                            btn = new Button();
                            btn.Name = btnCancel + i;
                            btn.Height = 20;
                            btn.Width = 45;
                            btn.HorizontalAlignment = HorizontalAlignment.Right;
                            btn.VerticalAlignment = VerticalAlignment.Top;
                            btn.Margin = new Thickness(0, i * 30 + 11, 30, 0);
                            btn.Content = "取消";
                            btn.Foreground = new SolidColorBrush(Colors.Black);
                            gridControl.Children.Add(btn);
                            btn.Click += btnCancel_Click; ;
                            btn.Visibility = Visibility.Hidden;
                            Dispatcher.Invoke(() => { processBar.Value += 100 / (double)pdfFileName.Length / 2; });
                            controlCollection.Add(btn);

                            dicControl.Add(index, controlCollection);
                        }
                    }

                    line.Name = "lineMark";
                    line.X1 = 0;
                    line.X2 = 780;
                    line.Y1 = 0;
                    line.Y2 = 0;
                    line.Stroke = new SolidColorBrush(Colors.AliceBlue);
                    line.StrokeThickness = 1;
                    line.HorizontalAlignment = HorizontalAlignment.Left;
                    line.VerticalAlignment = VerticalAlignment.Top;
                    gridControl.Children.Add(line);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var btnIndex = (sender as Button).Name.Replace(btnCancel, "");
            (dicControl[int.Parse(btnIndex)][0] as Label).Foreground = new SolidColorBrush(Colors.White);
            (dicControl[int.Parse(btnIndex)][0] as Label).Content = (dicControl[int.Parse(btnIndex)][0] as Label).Tag;
            (dicControl[int.Parse(btnIndex)][3] as Button).Visibility = Visibility.Hidden;
            (dicControl[int.Parse(btnIndex)][4] as Button).Visibility = Visibility.Hidden;
            (dicControl[int.Parse(btnIndex)][0] as Label).Tag = null;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btnIndex = (sender as Button).Name.Replace(btnSave, "");
                FileInfo fileInfo = new FileInfo((dicControl[int.Parse(btnIndex)][0] as Label).Tag.ToString());
                fileInfo.MoveTo((dicControl[int.Parse(btnIndex)][0] as Label).Content.ToString());
                (dicControl[int.Parse(btnIndex)][0] as Label).Foreground = new SolidColorBrush(Colors.White);
                (dicControl[int.Parse(btnIndex)][3] as Button).Visibility = Visibility.Hidden;
                (dicControl[int.Parse(btnIndex)][4] as Button).Visibility = Visibility.Hidden;
                (dicControl[int.Parse(btnIndex)][0] as Label).Tag = null;
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            var btnIndex = (sender as Button).Name.Replace(btnView, "");
            var path = (dicControl[int.Parse(btnIndex)][0] as Label).Tag == null
                ? (dicControl[int.Parse(btnIndex)][0] as Label).Content
                : (dicControl[int.Parse(btnIndex)][0] as Label).Tag;

            PreviewPdf preview = new PreviewPdf(path.ToString());
            preview.ShowDialog();
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            var originalPat = string.Empty;
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Interop.HwndSource source = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new PDFInvoice.folderBrowserDialog(source.Handle);

            if (dlg.ShowDialog(win) == System.Windows.Forms.DialogResult.OK)
            {
                var selectPath = dlg.SelectedPath;
                var btnIndex = (sender as Button).Name.Replace(btnModify, "");
                if (!(dicControl[int.Parse(btnIndex)][0] as Label).Content.ToString().Contains(selectPath))
                {
                    originalPat = (dicControl[int.Parse(btnIndex)][0] as Label).Content.ToString();
                    (dicControl[int.Parse(btnIndex)][3] as Button).Visibility = Visibility.Visible;
                    (dicControl[int.Parse(btnIndex)][4] as Button).Visibility = Visibility.Visible;
                    (dicControl[int.Parse(btnIndex)][0] as Label).Content = selectPath + System.IO.Path.GetFileName(originalPat);
                    (dicControl[int.Parse(btnIndex)][0] as Label).Tag = originalPat;
                    (dicControl[int.Parse(btnIndex)][0] as Label).Foreground = new SolidColorBrush(Colors.LightBlue);
                }
            }
        }

        private void Scrolls_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(scrolls);
            line.Margin = new Thickness(0, point.Y + 8, 0, 0);
        }
    }
}