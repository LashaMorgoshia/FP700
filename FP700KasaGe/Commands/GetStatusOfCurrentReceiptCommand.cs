using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class GetStatusOfCurrentReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public GetStatusOfCurrentReceiptCommand()
        {
            Command = 76;
            Data = string.Empty;
        }
    }
}
