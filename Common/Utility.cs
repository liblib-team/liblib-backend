using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Common
{
    public class Utility
    {

        public static string TranslateStatusToVI(string status)
        {
            switch (status)
            {
                case "Pending": return "Đang chờ";
                case "Accepted": return "Chấp nhận";
                case "Cancelled": return "Đã hủy";
                case "Rejected": return "Từ chối";
                case "Borrowing": return "Đang mượn";
                case "Returned": return "Đã trả";
                case "Overdue": return "Quá hạn";
            }
            return "";
        }

        public static bool Contains(string str1, string str2)
        {
            if (string.IsNullOrWhiteSpace(str2))
            {
                return true;
            }
            if (string.IsNullOrWhiteSpace(str1))
            {
                return false;
            }
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            return compareInfo.IndexOf(str1, str2, CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreNonSpace) > -1;
        }
    }
}
