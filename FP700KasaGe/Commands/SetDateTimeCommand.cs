using FP700KasaGe.Core;
using System;

namespace FP700KasaGe.Commands
{
    internal class SetDateTimeCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public SetDateTimeCommand(DateTime dateTime)
        {
            Command = 61;
            Data = dateTime.ToString("dd-MM-yy HH:mm:ss") + "\t";
        }
    }
}
