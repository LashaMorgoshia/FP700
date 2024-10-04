using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FP700KasaGe.Shared
{
    internal static class Extensions
    {
        private static readonly NumberFormatInfo _numFormatInfo;

        static Extensions()
        {
            _numFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
        }

        internal static string Merge(this IEnumerable<object> enumerable, string separator)
        {
            return string.Join("", enumerable.Select(delegate (object x)
            {
                string text = ((!(x.GetType() == typeof(decimal))) ? x.ToString() : ((decimal)x).ToString(_numFormatInfo));
                return text + separator;
            }).ToArray());
        }

        public static string ToString(this byte[] buffer)
        {
            return Encoding.GetEncoding(1251).GetString(buffer);
        }
    }

}
