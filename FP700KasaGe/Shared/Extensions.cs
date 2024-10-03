using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FP700KasaGe.Shared
{
    internal static class Extensions
    {
        private static readonly NumberFormatInfo Nfi;

        static Extensions()
        {
            Nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
        }

        internal static string StringJoin(this IEnumerable<object> enumerable, string separator)
        {
            return string.Join("", enumerable.Select(delegate (object x)
            {
                string text = ((!(x.GetType() == typeof(decimal))) ? x.ToString() : ((decimal)x).ToString(Nfi));
                return text + separator;
            }).ToArray());
        }

        public static string GetString(this byte[] buffer)
        {
            return Encoding.GetEncoding(1251).GetString(buffer);
        }
    }

}
