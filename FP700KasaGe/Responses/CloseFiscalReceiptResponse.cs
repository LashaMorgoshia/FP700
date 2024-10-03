using FP700KasaGe.Core;

namespace FP700KasaGe.Responses
{
    public class CloseFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt 
        /// </summary>
        public int SlipNumber { get; set; }

        /// <summary>
        /// Current slip number of this type: cash debit receipt or cash credit receipt or cashfree debit receipt or cashfree credit rceipt
        /// </summary>
        public int SlipNumberOfThisType { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public CloseFiscalReceiptResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                SlipNumber = int.Parse(values[0]);
                SlipNumberOfThisType = int.Parse(values[1]);
                DocNumber = int.Parse(values[2]);
            }
        }
    }

}
