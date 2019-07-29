using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using GetPdfMessage;
using Microsoft.Win32;
using Spire.Pdf;

namespace PDFInvoice.Screen
{
    /// <summary>
    /// GetSource.xaml 的交互逻辑
    /// </summary>
    public partial class GetSource : UserControl
    {
        private string[] pdfFileName { get; set; }
        private string pdfPath { get; set; }
        private string pdfText { get; set; } = string.Empty;
        private string CompamyCount { get; set; } = string.Empty;

        public GetSource()
        {
            InitializeComponent();
            Open.Click += Open_Click;
            Check.Click += Check_Click;
            tbDisplay.Text = string.Empty;
            this.DragEnter += Encode_DragEnter;
            radioButtonSource.Checked += RadioButtonSource_Checked;
            radioButtonCompany.Checked += RadioButtonCompany_Checked;
            this.Loaded += GetSource_Loaded;
        }

        private void GetSource_Loaded(object sender, RoutedEventArgs e)
        {
            radioButtonSource.Visibility = Visibility.Hidden;
            radioButtonCompany.Visibility = Visibility.Hidden;
        }

        private void RadioButtonSource_Checked(object sender, RoutedEventArgs e)
        {
            tbDisplay.Text = pdfText;
        }

        private void RadioButtonCompany_Checked(object sender, RoutedEventArgs e)
        {
            tbDisplay.Text = CompamyCount;
        }

        /// <summary>
        /// Drop file into application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Encode_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                var ss = file;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            radioButtonSource.Visibility = Visibility.Hidden;
            radioButtonCompany.Visibility = Visibility.Hidden;
            OpenFileDialog openFileDialog = new OpenFileDialog();
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
                tbDisplay.Text = $"Total:{pdfFileName.Length} files";
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            radioButtonSource.Visibility = Visibility.Hidden;
            radioButtonCompany.Visibility = Visibility.Hidden;
            processBar.Value = 0;
            Thread tr = new Thread(ReadSource);
            tr.Start();
            Open.IsEnabled = false;
            Check.IsEnabled = false;
        }

        private Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

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

        private void ReadSource()
        {
            try
            {
                double total = 0.0;
                double taxtotal = 0.0;
                dic.Clear();
                PdfDocument document = new PdfDocument();
                pdfText = string.Empty;
                CompamyCount = string.Empty;
                for (int i = 0; i < pdfFileName.Length; i++)
                {
                    document.LoadFromFile(pdfFileName[i]);

                    StringBuilder content = new StringBuilder();
                    content.Append(document.Pages[0].ExtractText());

                    string pdfSource = content.ToString().Trim();
                    GetPdfMsg getMsg = new GetPdfMsg();

                    //税号
                    var taxNumber = getMsg.GetInf(pdfSource, InvoiceParam.keywordType.TaxNumber);
                    //金额
                    var piceMark = getMsg.GetInf(pdfSource, InvoiceParam.keywordType.Amount);
                    //税额
                    var taxCount = getMsg.GetInf(pdfSource, InvoiceParam.keywordType.TaxCount);
                    //公司名称
                    var companyName = getMsg.GetInf(pdfSource, InvoiceParam.keywordType.CompanyName);

                    Statistics(companyName, pdfFileName[i]);

                    pdfText += "\r\n" + companyName + "\r\n" + "税号: " + taxNumber + "\t金额: " + piceMark + "\t税额: " + taxCount + "\r\n";

                    Dispatcher.Invoke(() => { tbDisplay.Text = "当前第：" + (i + 1) + "个" + "。共：" + pdfFileName.Length + "个" + pdfText; });

                    total += double.Parse(piceMark);
                    taxtotal += double.Parse(taxCount);
                    Dispatcher.Invoke(() => { processBar.Value += 100 / (double)pdfFileName.Length; });
                }
                Dispatcher.Invoke(() =>
                {
                    radioButtonSource.Visibility = Visibility.Visible;
                    radioButtonCompany.Visibility = Visibility.Visible;
                    radioButtonSource.IsChecked = true;
                });
                foreach (KeyValuePair<string, List<string>> dicitem in dic)
                {
                    var companyName = string.Empty;
                    for (int c = 0; c < dic[dicitem.Key].Count; c++)
                    {
                        companyName += dic[dicitem.Key][c] + "\r\n";
                    }
                    CompamyCount += "购买方: " + dicitem.Key + "\r\n" + companyName + ": 共" + dic[dicitem.Key].Count + "条\r\n\n";
                }

                Dispatcher.Invoke(() => { tbDisplay.Text += "\r\n完成"; });

                pdfText += "\r\n总金额: " + total.ToString() + "\t总税额: " + taxtotal.ToString();
                File.WriteAllText("Pdf.txt", pdfText);
                System.Diagnostics.Process.Start("Pdf.txt");

                for (int k = 0; k < pdfFileName.Length; k++)
                {
                    File.Delete(pdfFileName[k].Replace(".pdf", ".txt"));
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { tbDisplay.Text = ex.Message; });
                return;
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    Open.IsEnabled = true;
                    Check.IsEnabled = true;
                });
            }
        }
    }
}