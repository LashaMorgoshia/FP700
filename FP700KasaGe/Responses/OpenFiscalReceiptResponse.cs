using FP700KasaGe.Core;

namespace FP700KasaGe.Responses
{
    public class OpenFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt 
        /// </summary>
        public int SlipNumber { get; set; }

        public OpenFiscalReceiptResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                SlipNumber = int.Parse(values[0]);
            }
        }
    }
}
