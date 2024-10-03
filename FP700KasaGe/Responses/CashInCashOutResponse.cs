using FP700KasaGe.Core;
using System.Globalization;

namespace FP700KasaGe.Responses
{
    public class CashInCashOutResponse : FiscalResponse
    {
        /// <summary>
        /// Cash in safe sum 
        /// </summary>
        public decimal CashSum { get; set; }

        /// <summary>
        /// Total sum of cash in operations 
        /// </summary>
        public decimal CashIn { get; set; }

        /// <summary>
        /// Total sum of cash out operations 
        /// </summary>
        public decimal CashOut { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public CashInCashOutResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                CashSum = decimal.Parse(values[0], CultureInfo.InvariantCulture);
                CashIn = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                CashOut = decimal.Parse(values[2], CultureInfo.InvariantCulture);
                DocNumber = int.Parse(values[3]);
            }
        }
    }
}
