using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class AddTextToNonFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public AddTextToNonFiscalReceiptCommand(string text)
        {
            Command = 42;
            Data = text + "\t";
        }
    }
}
