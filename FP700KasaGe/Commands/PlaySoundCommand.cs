using FP700KasaGe.Core;
using FP700KasaGe.Shared;

namespace FP700KasaGe.Commands
{
    internal class PlaySoundCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public PlaySoundCommand(int frequency, int interval)
        {
            Command = 80;
            Data = new object[2] { frequency, interval }.Merge("\t");
        }
    }
}
