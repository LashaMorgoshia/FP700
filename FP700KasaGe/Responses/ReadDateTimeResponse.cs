using FP700KasaGe.Core;
using System;
using System.Globalization;

namespace FP700KasaGe.Responses
{
    public class ReadDateTimeResponse : FiscalResponse
    {
        /// <summary>
        /// Current Date and time in ECR
        /// </summary>
        public DateTime DateTime { get; set; }

        public ReadDateTimeResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                DateTime = DateTime.ParseExact(values[0], "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
            }
        }
    }
}
