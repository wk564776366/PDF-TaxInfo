using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFInvoice
{
    public class folderBrowserDialog : System.Windows.Forms.IWin32Window
    {
        private IntPtr _handle;

        public folderBrowserDialog(IntPtr handle)
        {
            _handle = handle;
        }

        IntPtr System.Windows.Forms.IWin32Window.Handle
        {
            get { return _handle; }
        }
    }
}