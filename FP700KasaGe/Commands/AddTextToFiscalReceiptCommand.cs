using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class AddTextToFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public AddTextToFiscalReceiptCommand(string text)
        {
            Command = 54;
            Data = text + "\t";
        }
    }

}
