using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class GetLastFiscalEntryInfoCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public GetLastFiscalEntryInfoCommand(int type)
        {
            Command = 64;
            Data = type + "\t";
        }
    }

}
