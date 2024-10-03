using FP700KasaGe.Core;

namespace FP700KasaGe.Commands
{
    internal class CalculateTotalCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public CalculateTotalCommand(int paymentMode)
        {
            Command = 53;
            Data = paymentMode + "\t\t";
        }
    }
}
