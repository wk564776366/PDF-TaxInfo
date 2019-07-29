using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PDFInvoice
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var GetNow = DateVerification.GetTime();
            DateTime effective = new DateTime(2020, 3, 27);
            if (GetNow >= effective)
            {
                try
                {
                    Application.Current.Shutdown();
                    throw new Exception("超出使用期限！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}