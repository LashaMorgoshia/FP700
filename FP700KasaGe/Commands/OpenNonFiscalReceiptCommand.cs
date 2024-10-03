using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class OpenNonFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public OpenNonFiscalReceiptCommand()
        {
            Command = 38;
            Data = string.Empty;
        }
    }
}
