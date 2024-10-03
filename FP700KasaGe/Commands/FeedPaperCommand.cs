using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class FeedPaperCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public FeedPaperCommand(int lines)
        {
            Command = 44;
            Data = lines + "\t";
        }
    }
}
