using FP700KasaGe.Core;
using FP700KasaGe.Shared;

namespace FP700KasaGe.Commands
{
    internal class OpenFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public OpenFiscalReceiptCommand(string opCode, string opPwd)
        {
            Command = 48;
            Data = new object[4]
            {
            opCode,
            opPwd,
            string.Empty,
            0
            }.Merge("\t");
        }

        public OpenFiscalReceiptCommand(string opCode, string opPwd, int type)
        {
            Command = 48;
            Data = new object[4]
            {
            opCode,
            opPwd,
            string.Empty,
            type
            }.Merge("\t");
        }

        public OpenFiscalReceiptCommand(string opCode, string opPwd, int type, int tillNumber)
        {
            Command = 48;
            Data = new object[4] { opCode, opPwd, tillNumber, type }.Merge("\t");
        }
    }
}
