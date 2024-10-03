using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FP700KasaGe.Core
{
    public interface IWrappedMessage
    {
        int Command { get; }

        string Data { get; }

        byte[] GetBytes(int sequence);
    }


    public abstract class WrappedMessage : IWrappedMessage
    {
        private readonly Dictionary<char, char> _geoToRusDict = new Dictionary<char, char>
    {
        { 'ა', 'а' },
        { 'ბ', 'б' },
        { 'გ', 'в' },
        { 'დ', 'г' },
        { 'ე', 'д' },
        { 'ვ', 'е' },
        { 'ზ', 'ж' },
        { 'თ', 'з' },
        { 'ი', 'и' },
        { 'კ', 'й' },
        { 'ლ', 'к' },
        { 'მ', 'л' },
        { 'ნ', 'м' },
        { 'ო', 'н' },
        { 'პ', 'о' },
        { 'ჟ', 'п' },
        { 'რ', 'р' },
        { 'ს', 'с' },
        { 'ტ', 'т' },
        { 'უ', 'у' },
        { 'ფ', 'ф' },
        { 'ქ', 'х' },
        { 'ღ', 'ц' },
        { 'ყ', 'ч' },
        { 'შ', 'ш' },
        { 'ჩ', 'щ' },
        { 'ც', 'ъ' },
        { 'ძ', 'ы' },
        { 'წ', 'ь' },
        { 'ჭ', 'э' },
        { 'ხ', 'ю' },
        { 'ჯ', 'я' },
        { 'ჰ', 'ё' }
    };

        public abstract int Command { get; }

        public abstract string Data { get; }

        public byte[] GetBytes(int sequence)
        {
            if (Data.Length > 213)
            {
                throw new InvalidDataException("Lenght of the packet exceeds the limits.");
            }
            int i = 0;
            string dataConverted = toAnsi(Data);
            int len = dataConverted.Length + 10;
            byte[] packet = new byte[len + 6];
            packet.SetValue((byte)1, i++);
            i = quarterize(packet, i, len + 32);
            packet.SetValue((byte)sequence, i++);
            i = quarterize(packet, i, Command);
            i = addData(packet, i, dataConverted);
            packet.SetValue((byte)5, i++);
            i = quarterize(packet, i, getChecksum(packet));
            packet.SetValue((byte)3, i);
            return packet;
        }

        private static int getChecksum(byte[] packet)
        {
            int result = 0;
            int indexOfPostamble = Array.IndexOf(packet, (byte)5);
            for (int i = 1; i <= indexOfPostamble; i++)
            {
                result += (byte)packet.GetValue(i);
            }
            return result;
        }

        private static int addData(byte[] buffer, int offset, string data)
        {
            byte[] bytes = Encoding.GetEncoding(1251).GetBytes(data);
            foreach (byte b in bytes)
            {
                buffer.SetValue(b, offset++);
            }
            return offset;
        }

        private static int quarterize(byte[] buffer, int offset, int value)
        {
            int[] array = new int[4] { 12, 8, 4, 0 };
            foreach (int shifter in array)
            {
                buffer.SetValue((byte)(((value >> shifter) & 0xF) + 48), offset++);
            }
            return offset;
        }

        private string toAnsi(string source)
        {
            string result = string.Empty;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                result = ((!_geoToRusDict.ContainsKey(c)) ? (result + c) : (result + _geoToRusDict[c]));
            }
            return result;
        }
    }

}
