using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class CloseNonFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public CloseNonFiscalReceiptCommand()
        {
            Command = 39;
            Data = string.Empty;
        }
    }
}
