using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class OpenDrawerCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public OpenDrawerCommand(int impulseLength)
        {
            Command = 106;
            Data = impulseLength + "\t";
        }
    }

}
