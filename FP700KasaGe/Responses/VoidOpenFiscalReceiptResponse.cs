using FP700KasaGe.Core;

namespace FP700KasaGe.Responses
{
    public class VoidOpenFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt
        /// </summary>
        public int SlipNumber { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public VoidOpenFiscalReceiptResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                SlipNumber = int.Parse(values[0]);
                DocNumber = int.Parse(values[1]);
            }
        }
    }
}
