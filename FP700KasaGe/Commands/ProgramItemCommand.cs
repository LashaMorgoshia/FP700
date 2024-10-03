using FP700KasaGe.Core;
using FP700KasaGe.Shared;

namespace FP700KasaGe.Commands
{
    internal class ProgramItemCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public ProgramItemCommand(string name, int plu, TaxGr taxGr, int dep, int group, decimal price, decimal quantity = 9999m, PriceType priceType = PriceType.FixedPrice)
        {
            Command = 107;
            Data = new object[14]
            {
            "P",
            plu,
            taxGr,
            dep,
            group,
            (int)priceType,
            price,
            "",
            quantity,
            "",
            "",
            "",
            "",
            name
            }.StringJoin("\t");
        }
    }
}
