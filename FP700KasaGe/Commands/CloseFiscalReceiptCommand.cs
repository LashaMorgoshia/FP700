using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class CloseFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public CloseFiscalReceiptCommand()
        {
            Command = 56;
            Data = string.Empty;
        }
    }

}
