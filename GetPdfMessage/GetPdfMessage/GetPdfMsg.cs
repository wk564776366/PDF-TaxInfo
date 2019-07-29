using System;
using System.Collections.Generic;
using System.Text;
using static GetPdfMessage.InvoiceParam;

namespace GetPdfMessage
{
    public class GetPdfMsg
    {
        private string GetNumberInfo { get; set; }
        private Dictionary<keywordType, string> keyValues = new Dictionary<keywordType, string>();

        public GetPdfMsg()
        {
            keyValues.Add(keywordType.Amount, "小写");
            keyValues.Add(keywordType.TaxNumber, "发票号码");
            keyValues.Add(keywordType.TaxCount, "价税合计");
            keyValues.Add(keywordType.CompanyName, "  称");
        }

        /// <summary>
        /// 从PDF中获得关键字对应的信息，仅支持人名币
        /// </summary>
        /// <param name="info">PDF信息</param>
        /// <param name="keywordType">关键字类型</param>
        /// <returns></returns>
        public string GetInf(string info, keywordType keywordType)
        {
            GetNumberInfo = string.Empty;
            var keyword = keyValues[keywordType];
            if (keywordType == keywordType.TaxCount)
            {
                if (info.Contains("免税"))
                {
                    return "0.00";
                }
                else
                {
                    return GetTaxCount(info, keyword);
                }
            }
            else if (keywordType == keywordType.CompanyName)
            {
                int textStart = info.IndexOf(keyword);
                if (textStart == -1)
                {
                    textStart = info.IndexOf("名称");
                }
                info = info.Substring(textStart, info.Length - textStart);
                int textEnd = info.IndexOf("  密  ");
                if (textEnd == -1)
                {
                    textEnd = info.IndexOf(" ");
                }
                info = info.Substring(keyword.Length, textEnd - keyword.Length).TrimEnd();
                info = info.Replace(":", "");
                info = info.Replace("：", "");

                return info;
            }
            else
            {
                int textStart = info.IndexOf(keyword);
                info = info.Substring(textStart + keyword.Length).Trim();
                info = GetNumber(info);
                return info;
            }
        }

        /// <summary>
        /// 获取税额
        /// </summary>
        /// <param name="cparam">文本信息</param>
        /// <param name="keyword">关键字</param>
        /// <returns>税额</returns>
        private string GetTaxCount(string cparam, string keyword)
        {
            var textStart = cparam.IndexOf(keyword);
            var textEnd = 0;
            cparam = cparam.Substring(0, textStart).Trim();
            for (int j = cparam.Length - 1; j > 0; j--)
            {
                if (char.IsNumber(cparam[j]) == false && cparam[j] != '.')
                {
                    break;
                }
                else
                {
                    textEnd = j;
                }
            }

            return cparam = cparam.Substring(textEnd, cparam.Length - textEnd);
        }

        /// <summary>
        /// 获取金额
        /// </summary>
        /// <param name="cparam"></param>
        /// <returns></returns>
        private string GetNumber(string cparam)
        {
            if (char.IsNumber(cparam[0]) || cparam[0] == '.')
            {
                GetNumberInfo += cparam[0];
                cparam = cparam.Substring(1, cparam.Length - 1);
                GetNumber(cparam);
                return GetNumberInfo;
            }
            else if (char.IsNumber(cparam[0]) == false && cparam[0] != '\r')
            {
                cparam = cparam.Substring(1, cparam.Length - 1);
                GetNumber(cparam);
                return GetNumberInfo;
            }
            else
            {
                return GetNumberInfo;
            }
        }
    }
}