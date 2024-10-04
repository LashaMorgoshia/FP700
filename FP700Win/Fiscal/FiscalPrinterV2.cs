using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace FiscalPrinter
{
    #region Enums

    /// <summary>
    /// Represents the type of cash operation.
    /// </summary>
    public enum CashOperation
    {
        /// <summary>
        /// Cash in operation.
        /// </summary>
        In = 0,

        /// <summary>
        /// Cash out operation.
        /// </summary>
        Out = 1
    }

    /// <summary>
    /// Represents the type of discount applied.
    /// </summary>
    public enum DiscountType
    {
        /// <summary>
        /// Surcharge by percentage.
        /// </summary>
        SurchargeByPercentage = 1,

        /// <summary>
        /// Discount by percentage.
        /// </summary>
        DiscountByPercentage = 2,

        /// <summary>
        /// Surcharge by sum.
        /// </summary>
        SurchargeBySum = 3,

        /// <summary>
        /// Discount by sum.
        /// </summary>
        DiscountBySum = 4
    }

    /// <summary>
    /// Represents the type of fiscal entry information.
    /// </summary>
    public enum FiscalEntryInfoType
    {
        /// <summary>
        /// Cash debit.
        /// </summary>
        CashDebit = 0,

        /// <summary>
        /// Cash credit.
        /// </summary>
        CashCredit = 1,

        /// <summary>
        /// Cash free debit.
        /// </summary>
        CashFreeDebit = 2,

        /// <summary>
        /// Cash free credit.
        /// </summary>
        CashFreeCredit = 3
    }

    /// <summary>
    /// Represents the payment mode.
    /// </summary>
    public enum PaymentMode
    {
        /// <summary>
        /// Cash payment.
        /// </summary>
        Cash = 0,

        /// <summary>
        /// Card payment.
        /// </summary>
        Card = 1,

        /// <summary>
        /// Credit payment.
        /// </summary>
        Credit = 2,

        /// <summary>
        /// Tare payment.
        /// </summary>
        Tare = 3
    }

    /// <summary>
    /// Represents the price type.
    /// </summary>
    public enum PriceType
    {
        /// <summary>
        /// Fixed price.
        /// </summary>
        FixedPrice = 0,

        /// <summary>
        /// Free price.
        /// </summary>
        FreePrice = 1,

        /// <summary>
        /// Maximum price.
        /// </summary>
        MaxPrice = 2
    }

    /// <summary>
    /// Represents the type of receipt.
    /// </summary>
    public enum ReceiptType
    {
        /// <summary>
        /// Sale receipt.
        /// </summary>
        Sale = 1,

        /// <summary>
        /// Return receipt.
        /// </summary>
        Return = 2
    }

    /// <summary>
    /// Represents the type of report.
    /// </summary>
    public enum ReportType
    {
        /// <summary>
        /// X report.
        /// </summary>
        X = 1,

        /// <summary>
        /// Z report.
        /// </summary>
        Z = 2
    }

    /// <summary>
    /// Represents the tax code.
    /// </summary>
    public enum TaxCode
    {
        /// <summary>
        /// Tax code A.
        /// </summary>
        A = 1,

        /// <summary>
        /// Tax code B.
        /// </summary>
        B = 2,

        /// <summary>
        /// Tax code C.
        /// </summary>
        C = 3
    }

    /// <summary>
    /// Represents the tax group.
    /// </summary>
    public enum TaxGroup
    {
        /// <summary>
        /// Tax group A.
        /// </summary>
        A = 0,

        /// <summary>
        /// Tax group B.
        /// </summary>
        B = 1,

        /// <summary>
        /// Tax group C.
        /// </summary>
        C = 2
    }

    #endregion

    #region Commands

    /// <summary>
    /// Base class for all fiscal printer commands.
    /// </summary>
    public abstract class FiscalCommand : IWrappedMessage
    {
        public abstract int Command { get; }
        public abstract string Data { get; }

        private static readonly Dictionary<char, char> GeorgianToRussianMap = new Dictionary<char, char>
        {
            // Mapping of Georgian characters to Russian equivalents
            { 'ა', 'а' }, { 'ბ', 'б' }, { 'გ', 'в' }, { 'დ', 'г' },
            { 'ე', 'д' }, { 'ვ', 'е' }, { 'ზ', 'ж' }, { 'თ', 'з' },
            { 'ი', 'и' }, { 'კ', 'й' }, { 'ლ', 'к' }, { 'მ', 'л' },
            { 'ნ', 'м' }, { 'ო', 'н' }, { 'პ', 'о' }, { 'ჟ', 'п' },
            { 'რ', 'р' }, { 'ს', 'с' }, { 'ტ', 'т' }, { 'უ', 'у' },
            { 'ფ', 'ф' }, { 'ქ', 'х' }, { 'ღ', 'ц' }, { 'ყ', 'ч' },
            { 'შ', 'ш' }, { 'ჩ', 'щ' }, { 'ც', 'ъ' }, { 'ძ', 'ы' },
            { 'წ', 'ь' }, { 'ჭ', 'э' }, { 'ხ', 'ю' }, { 'ჯ', 'я' },
            { 'ჰ', 'ё' }
        };

        public byte[] GetBytes(int sequence)
        {
            const int MaxDataLength = 213;

            if (Data.Length > MaxDataLength)
            {
                throw new InvalidDataException("Packet length exceeds the maximum allowed size.");
            }

            string convertedData = ConvertToAnsi(Data);
            int dataLength = convertedData.Length + 10;
            byte[] packet = new byte[dataLength + 6];

            int index = 0;
            packet[index++] = 0x01; // SOH

            index = WriteQuartet(packet, index, dataLength + 32);
            packet[index++] = (byte)sequence;
            index = WriteQuartet(packet, index, Command);

            index = AddData(packet, index, convertedData);

            packet[index++] = 0x05; // ENQ
            index = WriteQuartet(packet, index, CalculateChecksum(packet));
            packet[index++] = 0x03; // ETX

            return packet;
        }

        private int CalculateChecksum(byte[] packet)
        {
            int checksum = 0;
            int indexOfPostamble = Array.IndexOf(packet, (byte)0x05); // ENQ

            for (int i = 1; i <= indexOfPostamble; i++)
            {
                checksum += packet[i];
            }

            return checksum;
        }

        private int AddData(byte[] buffer, int offset, string data)
        {
            byte[] dataBytes = Encoding.GetEncoding(1251).GetBytes(data);
            Array.Copy(dataBytes, 0, buffer, offset, dataBytes.Length);
            return offset + dataBytes.Length;
        }

        private int WriteQuartet(byte[] buffer, int offset, int value)
        {
            int[] shifts = { 12, 8, 4, 0 };

            foreach (int shift in shifts)
            {
                buffer[offset++] = (byte)(((value >> shift) & 0xF) + 48);
            }

            return offset;
        }

        private string ConvertToAnsi(string source)
        {
            StringBuilder result = new StringBuilder(source.Length);

            foreach (char c in source)
            {
                if (GeorgianToRussianMap.TryGetValue(c, out char mappedChar))
                {
                    result.Append(mappedChar);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// Command to add text to a fiscal receipt.
    /// </summary>
    internal class AddTextToFiscalReceiptCommand : FiscalCommand
    {
        public override int Command => 54;
        public override string Data { get; }

        public AddTextToFiscalReceiptCommand(string text)
        {
            Data = text + "\t";
        }
    }

    /// <summary>
    /// Command to add text to a non-fiscal receipt.
    /// </summary>
    internal class AddTextToNonFiscalReceiptCommand : FiscalCommand
    {
        public override int Command => 42;
        public override string Data { get; }

        public AddTextToNonFiscalReceiptCommand(string text)
        {
            Data = text + "\t";
        }
    }

    /// <summary>
    /// Command to calculate total.
    /// </summary>
    internal class CalculateTotalCommand : FiscalCommand
    {
        public override int Command => 53;
        public override string Data { get; }

        public CalculateTotalCommand(int paymentMode)
        {
            Data = $"{paymentMode}\t\t";
        }
    }

    /// <summary>
    /// Command for cash in or cash out operation.
    /// </summary>
    internal class CashInCashOutCommand : FiscalCommand
    {
        public override int Command => 70;
        public override string Data { get; }

        public CashInCashOutCommand(int type, decimal amount)
        {
            Data = $"{type}\t{amount.ToString(CultureInfo.InvariantCulture)}\t";
        }
    }

    /// <summary>
    /// Command to close a fiscal receipt.
    /// </summary>
    internal class CloseFiscalReceiptCommand : FiscalCommand
    {
        public override int Command => 56;
        public override string Data => string.Empty;
    }

    /// <summary>
    /// Command to close a non-fiscal receipt.
    /// </summary>
    internal class CloseNonFiscalReceiptCommand : FiscalCommand
    {
        public override int Command => 39;
        public override string Data => string.Empty;
    }

    /// <summary>
    /// Command to feed paper.
    /// </summary>
    internal class FeedPaperCommand : FiscalCommand
    {
        public override int Command => 44;
        public override string Data { get; }

        public FeedPaperCommand(int lines)
        {
            Data = $"{lines}\t";
        }
    }

    /// <summary>
    /// Command to get the last fiscal entry information.
    /// </summary>
    internal class GetLastFiscalEntryInfoCommand : FiscalCommand
    {
        public override int Command => 64;
        public override string Data { get; }

        public GetLastFiscalEntryInfoCommand(int type)
        {
            Data = $"{type}\t";
        }
    }

    /// <summary>
    /// Command to get the status of the current receipt.
    /// </summary>
    internal class GetStatusOfCurrentReceiptCommand : FiscalCommand
    {
        public override int Command => 76;
        public override string Data => string.Empty;
    }

    /// <summary>
    /// Command to open the cash drawer.
    /// </summary>
    internal class OpenDrawerCommand : FiscalCommand
    {
        public override int Command => 106;
        public override string Data { get; }

        public OpenDrawerCommand(int impulseLength)
        {
            Data = $"{impulseLength}\t";
        }
    }

    /// <summary>
    /// Command to open a fiscal receipt.
    /// </summary>
    internal class OpenFiscalReceiptCommand : FiscalCommand
    {
        public override int Command => 48;
        public override string Data { get; }

        public OpenFiscalReceiptCommand(string opCode, string opPwd, int type = 1, int tillNumber = 0)
        {
            Data = $"{opCode}\t{opPwd}\t{tillNumber}\t{type}\t";
        }
    }

    /// <summary>
    /// Command to open a non-fiscal receipt.
    /// </summary>
    internal class OpenNonFiscalReceiptCommand : FiscalCommand
    {
        public override int Command => 38;
        public override string Data => string.Empty;
    }

    /// <summary>
    /// Command to play a sound.
    /// </summary>
    internal class PlaySoundCommand : FiscalCommand
    {
        public override int Command => 80;
        public override string Data { get; }

        public PlaySoundCommand(int frequency, int interval)
        {
            Data = $"{frequency}\t{interval}\t";
        }
    }

    /// <summary>
    /// Command to print a report.
    /// </summary>
    internal class PrintReportCommand : FiscalCommand
    {
        public override int Command => 69;
        public override string Data { get; }

        public PrintReportCommand(string type)
        {
            Data = $"{type}\t";
        }
    }

    /// <summary>
    /// Command to program an item.
    /// </summary>
    internal class ProgramItemCommand : FiscalCommand
    {
        public override int Command => 107;
        public override string Data { get; }

        public ProgramItemCommand(string name, int plu, TaxGroup taxGroup, int department, int group, decimal price, decimal quantity = 9999m, PriceType priceType = PriceType.FixedPrice)
        {
            Data = $"P\t{plu}\t{(int)taxGroup}\t{department}\t{group}\t{(int)priceType}\t{price.ToString(CultureInfo.InvariantCulture)}\t\t{quantity.ToString(CultureInfo.InvariantCulture)}\t\t\t\t\t{name}\t";
        }
    }

    /// <summary>
    /// Command to read date and time.
    /// </summary>
    internal class ReadDateTimeCommand : FiscalCommand
    {
        public override int Command => 62;
        public override string Data => string.Empty;
    }

    /// <summary>
    /// Command to read an error.
    /// </summary>
    internal class ReadErrorCommand : FiscalCommand
    {
        public override int Command => 100;
        public override string Data { get; }

        public ReadErrorCommand(string errorCode)
        {
            Data = $"{errorCode}\t";
        }
    }

    /// <summary>
    /// Command to read the status.
    /// </summary>
    internal class ReadStatusCommand : FiscalCommand
    {
        public override int Command => 74;
        public override string Data => string.Empty;
    }

    /// <summary>
    /// Command to register a programmed item sale.
    /// </summary>
    internal class RegisterProgrammedItemSaleCommand : FiscalCommand
    {
        public override int Command => 58;
        public override string Data { get; }

        public RegisterProgrammedItemSaleCommand(int pluCode, decimal quantity, decimal price, int discountType = 0, decimal discountValue = 0)
        {
            Data = discountType == 0
                ? $"{pluCode}\t{quantity.ToString(CultureInfo.InvariantCulture)}\t{price.ToString(CultureInfo.InvariantCulture)}\t"
                : $"{pluCode}\t{quantity.ToString(CultureInfo.InvariantCulture)}\t{price.ToString(CultureInfo.InvariantCulture)}\t{discountType}\t{discountValue.ToString(CultureInfo.InvariantCulture)}\t";
        }
    }

    /// <summary>
    /// Command to register a sale.
    /// </summary>
    internal class RegisterSaleCommand : FiscalCommand
    {
        public override int Command => 49;
        public override string Data { get; }

        public RegisterSaleCommand(string pluName, int taxCode, decimal price, int departmentNumber, decimal quantity, int discountType = 0, decimal discountValue = 0)
        {
            Data = discountType == 0
                ? $"{pluName}\t{taxCode}\t{price.ToString(CultureInfo.InvariantCulture)}\t{quantity.ToString(CultureInfo.InvariantCulture)}\t0\t\t{departmentNumber}\t"
                : $"{pluName}\t{taxCode}\t{price.ToString(CultureInfo.InvariantCulture)}\t{quantity.ToString(CultureInfo.InvariantCulture)}\t{discountType}\t{discountValue.ToString(CultureInfo.InvariantCulture)}\t{departmentNumber}\t";
        }
    }

    /// <summary>
    /// Command to set date and time.
    /// </summary>
    internal class SetDateTimeCommand : FiscalCommand
    {
        public override int Command => 61;
        public override string Data { get; }

        public SetDateTimeCommand(DateTime dateTime)
        {
            Data = dateTime.ToString("dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture) + "\t";
        }
    }

    /// <summary>
    /// Command to void an open fiscal receipt.
    /// </summary>
    internal class VoidOpenFiscalReceiptCommand : FiscalCommand
    {
        public override int Command => 60;
        public override string Data => string.Empty;
    }

    #endregion

    #region Responses

    /// <summary>
    /// Base class for all fiscal printer responses.
    /// </summary>
    public class FiscalResponse : IFiscalResponse
    {
        public bool CommandPassed => string.IsNullOrEmpty(ErrorCode);
        public string ErrorCode { get; protected set; }
        protected byte[] Data { get; private set; }

        public FiscalResponse(byte[] buffer)
        {
            if (buffer == null || buffer.Length < 27)
            {
                throw new IOException("Invalid packet received.");
            }

            if (buffer[0] != 0x01) // SOH
            {
                throw new IOException("Invalid packet received. Missing SOH.");
            }

            if (buffer[buffer.Length - 1] != 0x03) // ETX
            {
                throw new IOException("Invalid packet received. Missing ETX.");
            }

            int indexOfSeparator = Array.IndexOf(buffer, (byte)0x04); // EOT
            int indexOfPostamble = Array.IndexOf(buffer, (byte)0x05); // ENQ

            if (indexOfSeparator == -1 || indexOfPostamble == -1)
            {
                throw new IOException("Invalid packet received. Missing required separators.");
            }

            if (indexOfPostamble - indexOfSeparator != 9)
            {
                throw new IOException("Invalid packet received. Incorrect postamble length.");
            }

            byte[] dataBytes = buffer.Skip(10).Take(indexOfSeparator - 10).ToArray();
            if (dataBytes.Length < 2)
            {
                throw new IOException("Invalid packet received. Data section too short.");
            }

            if (dataBytes[0] != 48) // ASCII '0'
            {
                ErrorCode = Encoding.GetEncoding(1251).GetString(dataBytes);
            }
            else
            {
                Data = dataBytes.Skip(2).ToArray();
            }
        }

        protected string[] GetDataValues()
        {
            if (CommandPassed && Data != null)
            {
                string dataString = Encoding.GetEncoding(1251).GetString(Data);
                return dataString.Split('\t');
            }

            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Response for the AddTextToFiscalReceiptCommand.
    /// </summary>
    public class AddTextToFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt.
        /// </summary>
        public int SlipNumber { get; }

        /// <summary>
        /// Global number of all documents.
        /// </summary>
        public int DocumentNumber { get; }

        public AddTextToFiscalReceiptResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 2)
            {
                SlipNumber = int.Parse(values[0], CultureInfo.InvariantCulture);
                DocumentNumber = int.Parse(values[1], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the AddTextToNonFiscalReceiptCommand.
    /// </summary>
    public class CommonFiscalResponse : FiscalResponse
    {
        /// <summary>
        /// Global number of all documents.
        /// </summary>
        public int DocumentNumber { get; }

        public CommonFiscalResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 1)
            {
                DocumentNumber = int.Parse(values[0], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the CalculateTotalCommand.
    /// </summary>
    public class CalculateTotalResponse : FiscalResponse
    {
        /// <summary>
        /// Status of the payment: "D" for exact amount, "R" for change due.
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// The residual amount or change due.
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Current slip number.
        /// </summary>
        public int SlipNumber { get; }

        /// <summary>
        /// Global document number.
        /// </summary>
        public int DocumentNumber { get; }

        public CalculateTotalResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 4)
            {
                Status = values[0];
                Amount = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                SlipNumber = int.Parse(values[2], CultureInfo.InvariantCulture);
                DocumentNumber = int.Parse(values[3], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the CashInCashOutCommand.
    /// </summary>
    public class CashInCashOutResponse : FiscalResponse
    {
        /// <summary>
        /// Cash in safe sum.
        /// </summary>
        public decimal CashSum { get; }

        /// <summary>
        /// Total cash in operations sum.
        /// </summary>
        public decimal CashIn { get; }

        /// <summary>
        /// Total cash out operations sum.
        /// </summary>
        public decimal CashOut { get; }

        /// <summary>
        /// Global document number.
        /// </summary>
        public int DocumentNumber { get; }

        public CashInCashOutResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 4)
            {
                CashSum = decimal.Parse(values[0], CultureInfo.InvariantCulture);
                CashIn = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                CashOut = decimal.Parse(values[2], CultureInfo.InvariantCulture);
                DocumentNumber = int.Parse(values[3], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the CloseFiscalReceiptCommand.
    /// </summary>
    public class CloseFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number.
        /// </summary>
        public int SlipNumber { get; }

        /// <summary>
        /// Slip number of this type.
        /// </summary>
        public int SlipNumberOfThisType { get; }

        /// <summary>
        /// Global document number.
        /// </summary>
        public int DocumentNumber { get; }

        public CloseFiscalReceiptResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 3)
            {
                SlipNumber = int.Parse(values[0], CultureInfo.InvariantCulture);
                SlipNumberOfThisType = int.Parse(values[1], CultureInfo.InvariantCulture);
                DocumentNumber = int.Parse(values[2], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the GetLastFiscalEntryInfoCommand.
    /// </summary>
    public class GetLastFiscalEntryInfoResponse : FiscalResponse
    {
        /// <summary>
        /// Number of report.
        /// </summary>
        public int ReportNumber { get; }

        /// <summary>
        /// Turnover sum.
        /// </summary>
        public decimal Sum { get; }

        /// <summary>
        /// VAT amount.
        /// </summary>
        public decimal Vat { get; }

        /// <summary>
        /// Date of fiscal record.
        /// </summary>
        public DateTime Date { get; }

        public GetLastFiscalEntryInfoResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 4)
            {
                ReportNumber = int.Parse(values[0], CultureInfo.InvariantCulture);
                Sum = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                Vat = decimal.Parse(values[2], CultureInfo.InvariantCulture);
                Date = DateTime.ParseExact(values[3], "dd-MM-yy", CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the GetStatusOfCurrentReceiptCommand.
    /// </summary>
    public class GetStatusOfCurrentReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Status description.
        /// </summary>
        public string StatusDescription { get; }

        /// <summary>
        /// Number of items.
        /// </summary>
        public int Items { get; }

        /// <summary>
        /// Receipt amount.
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Sum paid.
        /// </summary>
        public decimal SumPaid { get; }

        /// <summary>
        /// Current slip number.
        /// </summary>
        public int SlipNumber { get; }

        /// <summary>
        /// Global document number.
        /// </summary>
        public int DocumentNumber { get; }

        public GetStatusOfCurrentReceiptResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 6)
            {
                StatusDescription = GetStatusDescription(values[0]);
                Items = int.Parse(values[1], CultureInfo.InvariantCulture);
                Amount = decimal.Parse(values[2], CultureInfo.InvariantCulture);
                SumPaid = decimal.Parse(values[3], CultureInfo.InvariantCulture);
                SlipNumber = int.Parse(values[4], CultureInfo.InvariantCulture);
                DocumentNumber = int.Parse(values[5], CultureInfo.InvariantCulture);
            }
        }

        //private string GetStatusDescription(string statusCode)
        //{
        //    return statusCode switch
        //    {
        //        "0" => "Receipt is closed.",
        //        "1" => "Sales receipt is open.",
        //        "2" => "Return receipt is open.",
        //        "3" => "Sales receipt is open and payment is executed.",
        //        "4" => "Return receipt is open and payment is executed.",
        //        "5" => "Receipt turned to a non-fiscal.",
        //        "6" => "Non-fiscal receipt is open.",
        //        _ => "Unknown status."
        //    };
        //}

        private string GetStatusDescription(string statusCode)
        {
            switch (statusCode)
            {
                case "0":
                    return "Receipt is closed.";
                case "1":
                    return "Sales receipt is open.";
                case "2":
                    return "Return receipt is open.";
                case "3":
                    return "Sales receipt is open and payment is executed.";
                case "4":
                    return "Return receipt is open and payment is executed.";
                case "5":
                    return "Receipt turned to a non-fiscal.";
                case "6":
                    return "Non-fiscal receipt is open.";
                default:
                    return "Unknown status.";
            }
        }
    }

    /// <summary>
    /// Response for the OpenFiscalReceiptCommand.
    /// </summary>
    public class OpenFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number.
        /// </summary>
        public int SlipNumber { get; }

        public OpenFiscalReceiptResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 1)
            {
                SlipNumber = int.Parse(values[0], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the PrintReportCommand.
    /// </summary>
    public class PrintReportResponse : FiscalResponse
    {
        /// <summary>
        /// Number of Z-report.
        /// </summary>
        public int ReportNumber { get; }

        /// <summary>
        /// Total turnover of VAT group X in debit receipts.
        /// </summary>
        public decimal TotalTurnoverX { get; }

        /// <summary>
        /// Total turnover of VAT group X in credit receipts.
        /// </summary>
        public decimal TotalNegativeTurnoverX { get; }

        public PrintReportResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 3)
            {
                ReportNumber = int.Parse(values[0], CultureInfo.InvariantCulture);
                TotalTurnoverX = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                TotalNegativeTurnoverX = decimal.Parse(values[2], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the ReadDateTimeCommand.
    /// </summary>
    public class ReadDateTimeResponse : FiscalResponse
    {
        /// <summary>
        /// Current date and time.
        /// </summary>
        public DateTime DateTime { get; }

        public ReadDateTimeResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 1)
            {
                DateTime = DateTime.ParseExact(values[0], "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the ReadErrorCommand.
    /// </summary>
    public class ReadErrorResponse : FiscalResponse
    {
        /// <summary>
        /// Error code.
        /// </summary>
        public string ErrorCodeValue { get; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string ErrorMessage { get; }

        public ReadErrorResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 2)
            {
                ErrorCodeValue = values[0];
                ErrorMessage = values[1];
            }
        }
    }

    /// <summary>
    /// Response for the ReadStatusCommand.
    /// </summary>
    public class ReadStatusResponse : FiscalResponse
    {
        /// <summary>
        /// List of status messages.
        /// </summary>
        public List<string> StatusMessages { get; } = new List<string>();

        public ReadStatusResponse(byte[] buffer) : base(buffer)
        {
            if (CommandPassed && Data != null && Data.Length >= 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    byte statusByte = Data[i];
                    string statusMessage = GetStatusMessage(statusByte, i);
                    if (!string.IsNullOrEmpty(statusMessage))
                    {
                        StatusMessages.Add(statusMessage);
                    }
                }
            }
        }

        private string GetStatusMessage(byte statusByte, int byteIndex)
        {
            switch (byteIndex)
            {
                case 0:
                    if ((statusByte & 0x20) > 0) return "General error.";
                    if ((statusByte & 0x02) > 0) return "Invalid command code.";
                    if ((statusByte & 0x01) > 0) return "Syntax error.";
                    break;
                case 1:
                    if ((statusByte & 0x02) > 0) return "Command not permitted.";
                    if ((statusByte & 0x01) > 0) return "Overflow during command execution.";
                    break;
                case 2:
                    if ((statusByte & 0x20) > 0) return "Non-fiscal receipt is open.";
                    if ((statusByte & 0x10) > 0) return "EJ nearly full.";
                    if ((statusByte & 0x08) > 0) return "Fiscal receipt is open.";
                    if ((statusByte & 0x04) > 0) return "EJ is full.";
                    if ((statusByte & 0x01) > 0) return "End of paper.";
                    break;
                case 4:
                    if ((statusByte & 0x20) > 0) return "Error in fiscal memory.";
                    if ((statusByte & 0x10) > 0) return "Fiscal memory is full.";
                    if ((statusByte & 0x01) > 0) return "Error writing to fiscal memory.";
                    break;
                case 5:
                    if ((statusByte & 0x10) > 0) return "VAT set at least once.";
                    if ((statusByte & 0x08) > 0) return "ECR is fiscalized.";
                    if ((statusByte & 0x02) > 0) return "Fiscal memory is formatted.";
                    break;
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// Response for the RegisterSaleCommand.
    /// </summary>
    public class RegisterSaleResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number.
        /// </summary>
        public int SlipNumber { get; }

        /// <summary>
        /// Global document number.
        /// </summary>
        public int DocumentNumber { get; }

        public RegisterSaleResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 2)
            {
                SlipNumber = int.Parse(values[0], CultureInfo.InvariantCulture);
                DocumentNumber = int.Parse(values[1], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for the VoidOpenFiscalReceiptCommand.
    /// </summary>
    public class VoidOpenFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number.
        /// </summary>
        public int SlipNumber { get; }

        /// <summary>
        /// Global document number.
        /// </summary>
        public int DocumentNumber { get; }

        public VoidOpenFiscalReceiptResponse(byte[] buffer) : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length >= 2)
            {
                SlipNumber = int.Parse(values[0], CultureInfo.InvariantCulture);
                DocumentNumber = int.Parse(values[1], CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Response for commands that return no additional data.
    /// </summary>
    public class EmptyFiscalResponse : FiscalResponse
    {
        public EmptyFiscalResponse(byte[] buffer) : base(buffer) { }
    }

    #endregion

    #region Interfaces

    /// <summary>
    /// Interface for fiscal responses.
    /// </summary>
    public interface IFiscalResponse
    {
        bool CommandPassed { get; }
        string ErrorCode { get; }
    }

    /// <summary>
    /// Interface for wrapped messages.
    /// </summary>
    public interface IWrappedMessage
    {
        int Command { get; }
        string Data { get; }
        byte[] GetBytes(int sequence);
    }

    #endregion

    #region Utilities

    internal static class Extensions
    {
        private static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo
        {
            NumberDecimalSeparator = "."
        };

        internal static string Merge(this IEnumerable<object> values, string separator)
        {
            return string.Join("", values.Select(value =>
            {
                if (value is decimal decimalValue)
                {
                    return decimalValue.ToString(NumberFormatInfo) + separator;
                }

                return value.ToString() + separator;
            }));
        }

        internal static string ToAnsiString(this byte[] buffer)
        {
            return Encoding.GetEncoding(1251).GetString(buffer);
        }
    }

    #endregion

    #region Exceptions

    /// <summary>
    /// Represents errors that occur during fiscal operations.
    /// </summary>
    public class FiscalException : IOException
    {
        public FiscalException(string message) : base(message) { }
    }

    #endregion

    /// <summary>
    /// Represents the fiscal printer FP700 and provides methods to interact with it.
    /// </summary>
    public class FP700 : IDisposable
    {
        private SerialPort _serialPort;
        private int _sequence = 32;
        private bool _isReadStatusExecuted;
        private readonly Queue<byte> _readBufferQueue = new Queue<byte>();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FP700"/> class with the specified serial port name.
        /// </summary>
        /// <param name="portName">The name of the serial port to use.</param>
        public FP700(string portName)
        {
            if (string.IsNullOrWhiteSpace(portName))
            {
                throw new ArgumentException("Port name cannot be null or whitespace.", nameof(portName));
            }

            _serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _serialPort.Open();
        }

        /// <summary>
        /// Changes the serial port used by the FP700 instance at runtime.
        /// </summary>
        /// <param name="portName">The name of the new serial port.</param>
        public void ChangePort(string portName)
        {
            if (string.IsNullOrWhiteSpace(portName))
            {
                throw new ArgumentException("Port name cannot be null or whitespace.", nameof(portName));
            }

            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }

            _serialPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _serialPort.Open();
        }

        /// <summary>
        /// Executes a custom command and returns the response.
        /// </summary>
        /// <typeparam name="T">The type of the response expected.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>The response from the fiscal printer.</returns>
        public T ExecuteCustomCommand<T>(FiscalCommand command) where T : FiscalResponse
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return (T)SendCommand(command, bytes => (FiscalResponse)Activator.CreateInstance(typeof(T), bytes));
        }

        /// <summary>
        /// Opens a non-fiscal text receipt.
        /// </summary>
        /// <returns>A response indicating the result of the operation.</returns>
        public FiscalResponse OpenNonFiscalReceipt()
        {
            var command = new OpenNonFiscalReceiptCommand();
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Adds text to an open non-fiscal receipt.
        /// </summary>
        /// <param name="text">The text to add (up to 30 characters).</param>
        /// <returns>A response indicating the result of the operation.</returns>
        public FiscalResponse AddTextToNonFiscalReceipt(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 30)
            {
                throw new ArgumentException("Text must be between 1 and 30 characters.", nameof(text));
            }

            var command = new AddTextToNonFiscalReceiptCommand(text);
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Closes an open non-fiscal receipt.
        /// </summary>
        /// <returns>A response indicating the result of the operation.</returns>
        public FiscalResponse CloseNonFiscalReceipt()
        {
            var command = new CloseNonFiscalReceiptCommand();
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Opens a fiscal receipt.
        /// </summary>
        /// <param name="operatorCode">The operator code.</param>
        /// <param name="operatorPassword">The operator password.</param>
        /// <param name="receiptType">The type of receipt to open.</param>
        /// <param name="tillNumber">Optional till number (1...999). Defaults to the logical number of the ECR in the workplace.</param>
        /// <returns>The response containing receipt information.</returns>
        public OpenFiscalReceiptResponse OpenFiscalReceipt(
            string operatorCode,
            string operatorPassword,
            ReceiptType receiptType = ReceiptType.Sale,
            int? tillNumber = null)
        {
            if (string.IsNullOrWhiteSpace(operatorCode))
            {
                throw new ArgumentException("Operator code cannot be null or whitespace.", nameof(operatorCode));
            }

            if (string.IsNullOrWhiteSpace(operatorPassword))
            {
                throw new ArgumentException("Operator password cannot be null or whitespace.", nameof(operatorPassword));
            }

            FiscalCommand command = tillNumber.HasValue
                ? new OpenFiscalReceiptCommand(operatorCode, operatorPassword, (int)receiptType, tillNumber.Value)
                : new OpenFiscalReceiptCommand(operatorCode, operatorPassword, (int)receiptType);

            return (OpenFiscalReceiptResponse)SendCommand(command, bytes => new OpenFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Registers a sale item in an open fiscal receipt.
        /// </summary>
        /// <param name="itemName">The name of the item (up to 32 characters).</param>
        /// <param name="price">The price of the item.</param>
        /// <param name="quantity">The quantity of the item.</param>
        /// <param name="departmentNumber">The department number (1 to 16).</param>
        /// <param name="taxCode">The tax code (default is TaxCode.A).</param>
        /// <returns>The response containing sale registration information.</returns>
        public RegisterSaleResponse RegisterSale(
            string itemName,
            decimal price,
            decimal quantity,
            int departmentNumber,
            TaxCode taxCode = TaxCode.A)
        {
            if (string.IsNullOrWhiteSpace(itemName) || itemName.Length > 32)
            {
                throw new ArgumentException("Item name must be between 1 and 32 characters.", nameof(itemName));
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            if (departmentNumber < 1 || departmentNumber > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(departmentNumber), "Department number must be between 1 and 16.");
            }

            var command = new RegisterSaleCommand(itemName, (int)taxCode, price, departmentNumber, quantity);
            return (RegisterSaleResponse)SendCommand(command, bytes => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Registers a programmed item sale in an open fiscal receipt.
        /// </summary>
        /// <param name="pluCode">The PLU code of the item (1 to 100000).</param>
        /// <param name="price">The price of the item.</param>
        /// <param name="quantity">The quantity of the item.</param>
        /// <returns>The response containing sale registration information.</returns>
        public RegisterSaleResponse RegisterProgrammedItemSale(int pluCode, decimal price, decimal quantity)
        {
            if (pluCode < 1 || pluCode > 100000)
            {
                throw new ArgumentOutOfRangeException(nameof(pluCode), "PLU code must be between 1 and 100000.");
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            var command = new RegisterProgrammedItemSaleCommand(pluCode, quantity, price);
            return (RegisterSaleResponse)SendCommand(command, bytes => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Applies total calculation and payment to an open fiscal receipt.
        /// </summary>
        /// <param name="paymentMode">The payment mode (default is PaymentMode.Cash).</param>
        /// <returns>The response containing total calculation information.</returns>
        public CalculateTotalResponse Total(PaymentMode paymentMode = PaymentMode.Cash)
        {
            var command = new CalculateTotalCommand((int)paymentMode);
            return (CalculateTotalResponse)SendCommand(command, bytes => new CalculateTotalResponse(bytes));
        }

        /// <summary>
        /// Voids an open fiscal receipt.
        /// </summary>
        /// <returns>The response containing void operation information.</returns>
        public VoidOpenFiscalReceiptResponse VoidOpenFiscalReceipt()
        {
            var command = new VoidOpenFiscalReceiptCommand();
            return (VoidOpenFiscalReceiptResponse)SendCommand(command, bytes => new VoidOpenFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Adds text to an open fiscal receipt.
        /// </summary>
        /// <param name="text">The text to add (up to 32 characters).</param>
        /// <returns>The response containing receipt information.</returns>
        public AddTextToFiscalReceiptResponse AddTextToFiscalReceipt(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 32)
            {
                throw new ArgumentException("Text must be between 1 and 32 characters.", nameof(text));
            }

            var command = new AddTextToFiscalReceiptCommand(text);
            return (AddTextToFiscalReceiptResponse)SendCommand(command, bytes => new AddTextToFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Closes an open fiscal receipt.
        /// </summary>
        /// <returns>The response containing receipt closing information.</returns>
        public CloseFiscalReceiptResponse CloseFiscalReceipt()
        {
            var command = new CloseFiscalReceiptCommand();
            return (CloseFiscalReceiptResponse)SendCommand(command, bytes => new CloseFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Gets information on the last fiscal entry.
        /// </summary>
        /// <param name="entryType">The type of fiscal entry information (default is CashDebit).</param>
        /// <returns>The response containing fiscal entry information.</returns>
        public GetLastFiscalEntryInfoResponse GetLastFiscalEntryInfo(FiscalEntryInfoType entryType = FiscalEntryInfoType.CashDebit)
        {
            var command = new GetLastFiscalEntryInfoCommand((int)entryType);
            return (GetLastFiscalEntryInfoResponse)SendCommand(command, bytes => new GetLastFiscalEntryInfoResponse(bytes));
        }

        /// <summary>
        /// Performs a cash in or cash out operation.
        /// </summary>
        /// <param name="operationType">The type of cash operation.</param>
        /// <param name="amount">The amount for the operation.</param>
        /// <returns>The response containing cash operation information.</returns>
        public CashInCashOutResponse CashInCashOutOperation(CashOperation operationType, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }

            var command = new CashInCashOutCommand((int)operationType, amount);
            return (CashInCashOutResponse)SendCommand(command, bytes => new CashInCashOutResponse(bytes));
        }

        /// <summary>
        /// Reads the status of the fiscal printer.
        /// </summary>
        /// <returns>The response containing status information.</returns>
        public ReadStatusResponse ReadStatus()
        {
            var command = new ReadStatusCommand();
            return (ReadStatusResponse)SendCommand(command, bytes => new ReadStatusResponse(bytes));
        }

        /// <summary>
        /// Feeds blank paper.
        /// </summary>
        /// <param name="lines">The number of lines to feed (1 to 99, default is 1).</param>
        /// <returns>The response indicating the result of the operation.</returns>
        public FiscalResponse FeedPaper(int lines = 1)
        {
            if (lines < 1 || lines > 99)
            {
                throw new ArgumentOutOfRangeException(nameof(lines), "Lines must be between 1 and 99.");
            }

            var command = new FeedPaperCommand(lines);
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Prints the buffer.
        /// </summary>
        /// <returns>The response indicating the result of the operation.</returns>
        public FiscalResponse PrintBuffer()
        {
            var command = new FeedPaperCommand(0);
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Reads an error code explanation from the fiscal printer.
        /// </summary>
        /// <param name="errorCode">The error code to explain.</param>
        /// <returns>The response containing error explanation.</returns>
        public ReadErrorResponse ReadError(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode))
            {
                throw new ArgumentException("Error code cannot be null or whitespace.", nameof(errorCode));
            }

            var command = new ReadErrorCommand(errorCode);
            return (ReadErrorResponse)SendCommand(command, bytes => new ReadErrorResponse(bytes));
        }

        /// <summary>
        /// Causes the fiscal printer to play a sound.
        /// </summary>
        /// <param name="frequency">The frequency of the sound in Hertz.</param>
        /// <param name="duration">The duration of the sound in milliseconds.</param>
        /// <returns>The response indicating the result of the operation.</returns>
        public FiscalResponse PlaySound(int frequency, int duration)
        {
            if (frequency <= 0)
            {
                throw new ArgumentException("Frequency must be greater than zero.", nameof(frequency));
            }

            if (duration <= 0)
            {
                throw new ArgumentException("Duration must be greater than zero.", nameof(duration));
            }

            var command = new PlaySoundCommand(frequency, duration);
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Prints an X or Z report.
        /// </summary>
        /// <param name="reportType">The type of report to print.</param>
        /// <returns>The response containing report information.</returns>
        public PrintReportResponse PrintReport(ReportType reportType)
        {
            var command = new PrintReportCommand(reportType.ToString());
            return (PrintReportResponse)SendCommand(command, bytes => new PrintReportResponse(bytes));
        }

        /// <summary>
        /// Opens the cash drawer.
        /// </summary>
        /// <param name="impulseLength">The length of the impulse in milliseconds.</param>
        /// <returns>The response indicating the result of the operation.</returns>
        public FiscalResponse OpenDrawer(int impulseLength)
        {
            if (impulseLength <= 0)
            {
                throw new ArgumentException("Impulse length must be greater than zero.", nameof(impulseLength));
            }

            var command = new OpenDrawerCommand(impulseLength);
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Sets the date and time of the fiscal printer.
        /// </summary>
        /// <param name="dateTime">The date and time to set.</param>
        /// <returns>The response indicating the result of the operation.</returns>
        public FiscalResponse SetDateTime(DateTime dateTime)
        {
            var command = new SetDateTimeCommand(dateTime);
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Reads the current date and time from the fiscal printer.
        /// </summary>
        /// <returns>The response containing date and time information.</returns>
        public ReadDateTimeResponse ReadDateTime()
        {
            var command = new ReadDateTimeCommand();
            return (ReadDateTimeResponse)SendCommand(command, bytes => new ReadDateTimeResponse(bytes));
        }

        /// <summary>
        /// Gets the status of the current or last receipt.
        /// </summary>
        /// <returns>The response containing receipt status information.</returns>
        public GetStatusOfCurrentReceiptResponse GetStatusOfCurrentReceipt()
        {
            var command = new GetStatusOfCurrentReceiptCommand();
            return (GetStatusOfCurrentReceiptResponse)SendCommand(command, bytes => new GetStatusOfCurrentReceiptResponse(bytes));
        }

        /// <summary>
        /// Programs an item into the fiscal printer.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="plu">The PLU code of the item.</param>
        /// <param name="taxGroup">The tax group of the item.</param>
        /// <param name="department">The department number.</param>
        /// <param name="group">The group number.</param>
        /// <param name="price">The price of the item.</param>
        /// <param name="quantity">The quantity of the item (default is 9999).</param>
        /// <param name="priceType">The price type (default is FixedPrice).</param>
        /// <returns>The response indicating the result of the operation.</returns>
        public FiscalResponse ProgramItem(
            string name,
            int plu,
            TaxGroup taxGroup,
            int department,
            int group,
            decimal price,
            decimal quantity = 9999m,
            PriceType priceType = PriceType.FixedPrice)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Item name cannot be null or whitespace.", nameof(name));
            }

            if (plu < 1)
            {
                throw new ArgumentException("PLU code must be greater than zero.", nameof(plu));
            }

            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }

            var command = new ProgramItemCommand(name, plu, taxGroup, department, group, price, quantity, priceType);
            return SendCommand(command, bytes => new FiscalResponse(bytes));
        }

        /// <summary>
        /// Releases all resources used by the <see cref="FP700"/> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private T SendCommand<T>(FiscalCommand command, Func<byte[], T> responseFactory) where T : IFiscalResponse
        {
            if (!_isReadStatusExecuted)
            {
                SendCommand(new ReadStatusCommand(), bytes => (IFiscalResponse)null);
                _isReadStatusExecuted = true;
            }

            return SendFiscalMessage(command, responseFactory);
        }

        private T SendFiscalMessage<T>(FiscalCommand command, Func<byte[], T> responseFactory) where T : IFiscalResponse
        {
            T response = default;
            byte[] lastStatusBytes = null;
            byte[] packet = command.GetBytes(_sequence);

            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    _serialPort.Write(packet, 0, packet.Length);

                    List<byte> receivedBytes = new List<byte>();
                    while (ReadByte())
                    {
                        byte b = _readBufferQueue.Dequeue();
                        if (b == 0x15) // NAK
                        {
                            throw new IOException("Invalid packet checksum or message format.");
                        }

                        if (b == 0x16) // SYN
                        {
                            continue;
                        }

                        receivedBytes.Add(b);
                    }

                    receivedBytes.Add(_readBufferQueue.Dequeue());
                    byte[] buffer = receivedBytes.ToArray();
                    response = responseFactory(buffer);

                    int statusStartIndex = receivedBytes.IndexOf(0x04) + 1;
                    lastStatusBytes = receivedBytes.Skip(statusStartIndex).Take(6).ToArray();

                    break;
                }
                catch (Exception)
                {
                    if (attempt >= 2)
                    {
                        throw;
                    }

                    _readBufferQueue.Clear();
                    continue;
                }
            }

            _sequence++;
            if (_sequence > 254)
            {
                _sequence = 32;
            }

            if (command.Command != 74)
            {
                CheckStatusForErrors(lastStatusBytes);
            }

            return response;
        }

        private bool ReadByte()
        {
            int byteRead = _serialPort.ReadByte();
            _readBufferQueue.Enqueue((byte)byteRead);
            return byteRead != 0x03; // ETX
        }

        private void CheckStatusForErrors(byte[] statusBytes)
        {
            if (statusBytes == null || statusBytes.Length == 0)
            {
                throw new FiscalException("Invalid status bytes received.");
            }

            // Error checking based on status bytes as per device protocol
            if ((statusBytes[0] & 0x20) > 0)
            {
                throw new FiscalException("General error - combination of all marked errors.");
            }
            if ((statusBytes[0] & 0x02) > 0)
            {
                throw new FiscalException("Invalid command code.");
            }
            if ((statusBytes[0] & 0x01) > 0)
            {
                throw new FiscalException("Syntax error.");
            }
            if ((statusBytes[1] & 0x02) > 0)
            {
                throw new FiscalException("Command not permitted.");
            }
            if ((statusBytes[1] & 0x01) > 0)
            {
                throw new FiscalException("Overflow during command execution.");
            }
            if ((statusBytes[2] & 0x01) > 0)
            {
                throw new FiscalException("End of paper.");
            }
            if ((statusBytes[4] & 0x20) > 0)
            {
                throw new FiscalException("Combination of all marked errors.");
            }
            if ((statusBytes[4] & 0x10) > 0)
            {
                throw new FiscalException("Fiscal memory is full.");
            }
            if ((statusBytes[4] & 0x01) > 0)
            {
                throw new FiscalException("Error writing to fiscal memory.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _serialPort?.Dispose();
                }

                _disposed = true;
            }
        }

        ~FP700()
        {
            Dispose(false);
        }
    }
}