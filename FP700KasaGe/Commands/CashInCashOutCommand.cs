using FP700KasaGe.Core;
using FP700KasaGe.Shared;

namespace FP700KasaGe.Commands
{
    internal class CashInCashOutCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public CashInCashOutCommand(int type, decimal amount)
        {
            Command = 70;
            Data = new object[2] { type, amount }.StringJoin("\t");
        }
    }
}
