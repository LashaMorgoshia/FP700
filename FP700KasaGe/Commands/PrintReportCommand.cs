using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class PrintReportCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public PrintReportCommand(string type)
        {
            Command = 69;
            Data = type + "\t";
        }
    }

}
