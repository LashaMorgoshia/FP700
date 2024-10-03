using FP700KasaGe.Shared;
using System;
using System.IO;
using System.Linq;

namespace FP700KasaGe.Core
{
    public interface IFiscalResponse
    {
        bool CommandPassed { get; }

        string ErrorCode { get; set; }
    }

    public abstract class FiscalResponse : IFiscalResponse
    {
        public bool CommandPassed => string.IsNullOrEmpty(ErrorCode);

        public string ErrorCode { get; set; }

        protected byte[] Data { get; set; }

        protected FiscalResponse(byte[] buffer)
        {
            IOException invalidPacketException = new IOException("Invalid packet received.");
            if (buffer.Length < 27)
            {
                throw invalidPacketException;
            }
            if (buffer[0] != 1)
            {
                throw invalidPacketException;
            }
            if (buffer[buffer.Length - 1] != 3)
            {
                throw invalidPacketException;
            }
            int indexOfSeparator = Array.IndexOf(buffer, (byte)4);
            int indexOfPostamble = Array.IndexOf(buffer, (byte)5);
            if (indexOfSeparator == -1 || indexOfPostamble == -1)
            {
                throw invalidPacketException;
            }
            if (indexOfPostamble - indexOfSeparator != 9)
            {
                throw invalidPacketException;
            }
            byte[] dataBytes = buffer.Skip(10).Take(indexOfSeparator - 10).ToArray();
            if (dataBytes.Length < 2)
            {
                throw invalidPacketException;
            }
            if (dataBytes.First() != 48)
            {
                ErrorCode = dataBytes.GetString();
            }
            else
            {
                Data = dataBytes.Skip(2).Take(dataBytes.Length - 2).ToArray();
            }
        }

        protected string[] GetDataValues()
        {
            if (CommandPassed)
            {
                return Data.GetString().Split('\t');
            }
            return new string[0];
        }
    }

}
