using FP700KasaGe.Core;
using FP700KasaGe.Shared;

namespace FP700KasaGe.Commands
{
    internal class RegisterProgrammedItemSaleCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public RegisterProgrammedItemSaleCommand(int pluCode, decimal qty, decimal price)
        {
            Command = 58;
            Data = new object[3] { pluCode, qty, price }.StringJoin("\t");
        }

        public RegisterProgrammedItemSaleCommand(int pluCode, decimal qty, decimal price, int discountType, decimal discountValue)
        {
            Command = 58;
            Data = new object[5] { pluCode, qty, price, discountType, discountValue }.StringJoin("\t");
        }
    }

}
