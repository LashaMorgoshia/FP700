using System.IO;

namespace FP700KasaGe.Core
{
    public class FiscalIOException : IOException
    {
        public FiscalIOException(string message)
            : base(message)
        {
        }
    }

}
