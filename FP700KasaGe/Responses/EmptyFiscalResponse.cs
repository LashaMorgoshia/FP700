using FP700KasaGe.Core;

namespace FP700KasaGe.Responses
{
    public class EmptyFiscalResponse : FiscalResponse
    {
        public EmptyFiscalResponse(byte[] buffer)
            : base(buffer)
        {
        }
    }

}
