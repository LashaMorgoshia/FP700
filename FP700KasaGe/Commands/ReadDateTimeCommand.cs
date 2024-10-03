using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class ReadDateTimeCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public ReadDateTimeCommand()
        {
            Command = 62;
            Data = string.Empty;
        }
    }
}
