using FP700KasaGe.Core;
using System.Globalization;

namespace FP700KasaGe.Responses
{
    public class PrintReportResponse : FiscalResponse
    {
        /// <summary>
        /// Number of Z-report 
        /// </summary>
        public int nRep { get; set; }

        /// <summary>
        /// Turnovers of VAT group X in debit (cash and cashfree) receipts 
        /// </summary>
        public decimal TotX { get; set; }

        /// <summary>
        /// Turnovers of VAT group X in credit (cash and cashfree) receipts 
        /// </summary>
        public decimal TotNegX { get; set; }

        public PrintReportResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                nRep = int.Parse(values[0]);
                TotX = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                TotNegX = decimal.Parse(values[2], CultureInfo.InvariantCulture);
            }
        }
    }
}
