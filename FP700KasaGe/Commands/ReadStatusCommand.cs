using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class ReadStatusCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public ReadStatusCommand()
        {
            Command = 74;
            Data = string.Empty;
        }
    }
}
