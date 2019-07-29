using System;
using System.Collections.Generic;
using System.Text;

namespace GetPdfMessage
{
    public class InvoiceParam
    {
        public enum keywordType
        {
            //金额
            Amount,

            //发票号
            TaxNumber,

            //税额
            TaxCount,

            //公司名称
            CompanyName
        }
    }
}
