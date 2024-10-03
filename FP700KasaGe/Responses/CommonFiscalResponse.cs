using FP700KasaGe.Core;

namespace FP700KasaGe.Responses
{
    public class CommonFiscalResponse : FiscalResponse
    {
        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public CommonFiscalResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                DocNumber = int.Parse(values[0]);
            }
        }
    }
}
