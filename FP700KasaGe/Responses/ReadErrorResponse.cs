using FP700KasaGe.Core;

namespace FP700KasaGe.Responses
{
    public class ReadErrorResponse : FiscalResponse
    {
        /// <summary>
        /// Code of the error, to be explained; 
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Explanation of the error Code; 
        /// </summary>
        public string ErrorMessage { get; set; }

        public ReadErrorResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                Code = values[0];
                ErrorMessage = values[1];
            }
        }
    }
}
