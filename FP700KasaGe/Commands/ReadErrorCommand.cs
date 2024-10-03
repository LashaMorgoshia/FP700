using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class ReadErrorCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public ReadErrorCommand(string errorCode)
        {
            Command = 100;
            Data = errorCode + "\t";
        }
    }
}
