using FP700KasaGe.Core;
using FP700KasaGe.Shared;

namespace FP700KasaGe.Commands
{
    internal class RegisterSaleCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public RegisterSaleCommand(string pluName, int taxCode, decimal price, int departmentNumber, decimal qty)
        {
            Command = 49;
            Data = new object[7]
            {
            pluName,
            taxCode,
            price,
            qty,
            0,
            string.Empty,
            departmentNumber
            }.StringJoin("\t");
        }

        public RegisterSaleCommand(string pluName, int taxCode, decimal price, int departmentNumber, decimal qty, int discountType, decimal discountValue)
        {
            Command = 49;
            Data = new object[7] { pluName, taxCode, price, qty, discountType, discountValue, departmentNumber }.StringJoin("\t");
        }
    }
}
