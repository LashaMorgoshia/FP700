using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class VoidOpenFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public VoidOpenFiscalReceiptCommand()
        {
            Command = 60;
            Data = string.Empty;
        }
    }
}
