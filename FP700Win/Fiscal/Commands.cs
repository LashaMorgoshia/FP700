using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
    public abstract class FiscalCommand : IWrappedMessage
    {
        public abstract int Command { get; }
        public string Data { get; protected set; }

        protected FiscalCommand(string[] parameters)
        {
            Data = string.Join("\t", parameters);
        }

        public byte[] GetBytes(int sequence)
        {
            // Common logic for generating the byte array for commands
            byte[] packet = new byte[256];
            // Add logic to fill packet based on command and data
            return packet;
        }
    }

    internal class AddTextCommand : FiscalCommand
    {
        public override int Command { get; }

        public AddTextCommand(string text, bool isFiscal) : base(new[] { text })
        {
            Command = isFiscal ? 54 : 42;
        }
    }

    //internal class AddTextToFiscalReceiptCommand : WrappedMessage
    //{
    //    public override int Command { get; }

    //    public override string Data { get; }

    //    public AddTextToFiscalReceiptCommand(string text)
    //    {
    //        Command = 54;
    //        Data = text + "\t";
    //    }
    //}

    internal class AddTextToNonFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public AddTextToNonFiscalReceiptCommand(string text)
        {
            Command = 42;
            Data = text + "\t";
        }
    }

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

    internal class CashInCashOutCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public CashInCashOutCommand(int type, decimal amount)
        {
            Command = 70;
            Data = new object[2] { type, amount }.Merge("\t");
        }
    }

    internal class CloseFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public CloseFiscalReceiptCommand()
        {
            Command = 56;
            Data = string.Empty;
        }
    }

    internal class CloseNonFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public CloseNonFiscalReceiptCommand()
        {
            Command = 39;
            Data = string.Empty;
        }
    }

    internal class FeedPaperCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public FeedPaperCommand(int lines)
        {
            Command = 44;
            Data = lines + "\t";
        }
    }

    internal class GetLastFiscalEntryInfoCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public GetLastFiscalEntryInfoCommand(int type)
        {
            Command = 64;
            Data = type + "\t";
        }
    }

    internal class GetStatusOfCurrentReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public GetStatusOfCurrentReceiptCommand()
        {
            Command = 76;
            Data = string.Empty;
        }
    }

    internal class OpenDrawerCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public OpenDrawerCommand(int impulseLength)
        {
            Command = 106;
            Data = impulseLength + "\t";
        }
    }

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

    internal class OpenNonFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public OpenNonFiscalReceiptCommand()
        {
            Command = 38;
            Data = string.Empty;
        }
    }

    internal class PlaySoundCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public PlaySoundCommand(int frequency, int interval)
        {
            Command = 80;
            Data = new object[2] { frequency, interval }.Merge("\t");
        }
    }

    internal class PrintReportCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public PrintReportCommand(string type)
        {
            Command = 69;
            Data = type + "\t";
        }
    }

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
            }.Merge("\t");
        }
    }

    internal class ReadDateTimeCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public ReadDateTimeCommand()
        {
            Command = 62;
            Data = string.Empty;
        }
    }

    internal class ReadErrorCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public ReadErrorCommand(string errorCode)
        {
            Command = 100;
            Data = errorCode + "\t";
        }
    }

    internal class ReadStatusCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public ReadStatusCommand()
        {
            Command = 74;
            Data = string.Empty;
        }
    }

    internal class RegisterProgrammedItemSaleCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public RegisterProgrammedItemSaleCommand(int pluCode, decimal qty, decimal price)
        {
            Command = 58;
            Data = new object[3] { pluCode, qty, price }.Merge("\t");
        }

        public RegisterProgrammedItemSaleCommand(int pluCode, decimal qty, decimal price, int discountType, decimal discountValue)
        {
            Command = 58;
            Data = new object[5] { pluCode, qty, price, discountType, discountValue }.Merge("\t");
        }
    }

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
            }.Merge("\t");
        }

        public RegisterSaleCommand(string pluName, int taxCode, decimal price, int departmentNumber, decimal qty, int discountType, decimal discountValue)
        {
            Command = 49;
            Data = new object[7] { pluName, taxCode, price, qty, discountType, discountValue, departmentNumber }.Merge("\t");
        }
    }

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

    internal class VoidOpenFiscalReceiptCommand : WrappedMessage
    {
        public override int Command { get; }

        public override string Data { get; }

        public VoidOpenFiscalReceiptCommand()
        {
            Command = 60;
            Data = string.Empty;
        }
    }
    #endregion

    #region Responses
    public class AddTextToFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt
        /// </summary>
        public int SlipNumber { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public AddTextToFiscalReceiptResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                SlipNumber = int.Parse(values[0]);
                DocNumber = int.Parse(values[1]);
            }
        }
    }

    public class CalculateTotalResponse : FiscalResponse
    {
        /// <summary>
        /// "D" - when the paid sum is equal to the sum of the receipt. The residual sum due for payment (equal to 0) holds Amount property;
        /// "R" - when the paid sum is greater than the sum of the receipt. Amount property holds the Change; 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The residual sum due for payment (equal to 0) or the change; 
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Current slip number - unique number of the fiscal receipt
        /// </summary>
        public int SlipNumber { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public CalculateTotalResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                Status = values[0];
                Amount = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                SlipNumber = int.Parse(values[2]);
                DocNumber = int.Parse(values[3]);
            }
        }
    }

    public class CashInCashOutResponse : FiscalResponse
    {
        /// <summary>
        /// Cash in safe sum 
        /// </summary>
        public decimal CashSum { get; set; }

        /// <summary>
        /// Total sum of cash in operations 
        /// </summary>
        public decimal CashIn { get; set; }

        /// <summary>
        /// Total sum of cash out operations 
        /// </summary>
        public decimal CashOut { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public CashInCashOutResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                CashSum = decimal.Parse(values[0], CultureInfo.InvariantCulture);
                CashIn = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                CashOut = decimal.Parse(values[2], CultureInfo.InvariantCulture);
                DocNumber = int.Parse(values[3]);
            }
        }
    }

    public class CloseFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt 
        /// </summary>
        public int SlipNumber { get; set; }

        /// <summary>
        /// Current slip number of this type: cash debit receipt or cash credit receipt or cashfree debit receipt or cashfree credit rceipt
        /// </summary>
        public int SlipNumberOfThisType { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public CloseFiscalReceiptResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                SlipNumber = int.Parse(values[0]);
                SlipNumberOfThisType = int.Parse(values[1]);
                DocNumber = int.Parse(values[2]);
            }
        }
    }

    public class CommonFiscalResponse : FiscalResponse
    {
        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public CommonFiscalResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                DocNumber = int.Parse(values[0]);
            }
        }
    }

    public class EmptyFiscalResponse : FiscalResponse
    {
        public EmptyFiscalResponse(byte[] buffer)
            : base(buffer)
        {
        }
    }

    public class GetLastFiscalEntryInfoResponse : FiscalResponse
    {
        /// <summary>
        /// Number of report (1-3800)
        /// </summary>
        public int nRep { get; set; }

        /// <summary>
        /// Turnover in receipts of type defined by FiscalEntryInfoType
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// Vat amount in receipts of type defined by FiscalEntryInfoType 
        /// </summary>
        public decimal Vat { get; set; }

        /// <summary>
        /// Date of fiscal record 
        /// </summary>
        public DateTime Date { get; set; }

        public GetLastFiscalEntryInfoResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                nRep = int.Parse(values[0]);
                Sum = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                Vat = decimal.Parse(values[2], CultureInfo.InvariantCulture);
                Date = DateTime.ParseExact(values[3], "dd-MM-yy", CultureInfo.InvariantCulture);
            }
        }
    }

    public class GetStatusOfCurrentReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Status value of current or last receipt
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///  Number of sales in current or last fiscal receipt;
        /// </summary>
        public int Items { get; set; }

        /// <summary>
        /// The sum of current or last receipt;
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The sum paid in current or last receipt 
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// Current slip number - unique number of the fiscal receipt ( 1...99999999 ).
        /// Returned only if the receipt is open and is fiscal (Type = 1..5); 
        /// </summary>
        public int SlipNumber { get; set; }

        /// <summary>
        /// Global number of all documents;
        /// </summary>
        public int DocNumber { get; set; }

        public GetStatusOfCurrentReceiptResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                switch (values[0])
                {
                    case "0":
                        Value = "Receipt is closed;";
                        break;
                    case "1":
                        Value = "Sales receipt is open;";
                        break;
                    case "2":
                        Value = "Return receipt is open;";
                        break;
                    case "3":
                        Value = "Sales receipt is open and payment is executed (already closed in SAM) - closing command (command 56) should be used;";
                        break;
                    case "4":
                        Value = "Return receipt is open and payment is executed (already closed in SAM) - closing command (command 56) should be used;";
                        break;
                    case "5":
                        Value = "Sales or return receipt was open, but all void is executed and receipt is turned to a non fiscal - closing command (commands 39 or 56) should be used;";
                        break;
                    case "6":
                        Value = "Non fiscal receipt is open - closing command (command 39) should be used;";
                        break;
                    default:
                        Value = "Unknown";
                        break;
                }
                Items = int.Parse(values[1]);
                Amount = decimal.Parse(values[2], CultureInfo.InvariantCulture);
                Sum = decimal.Parse(values[3], CultureInfo.InvariantCulture);
                SlipNumber = int.Parse(values[4]);
                DocNumber = int.Parse(values[5]);
            }
        }
    }

    public class OpenFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt 
        /// </summary>
        public int SlipNumber { get; set; }

        public OpenFiscalReceiptResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                SlipNumber = int.Parse(values[0]);
            }
        }
    }

    public class PrintReportResponse : FiscalResponse
    {
        /// <summary>
        /// Number of Z-report 
        /// </summary>
        public int nRep { get; set; }

        /// <summary>
        /// Turnovers of VAT group X in debit (cash and cashfree) receipts 
        /// </summary>
        public decimal TotX { get; set; }

        /// <summary>
        /// Turnovers of VAT group X in credit (cash and cashfree) receipts 
        /// </summary>
        public decimal TotNegX { get; set; }

        public PrintReportResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                nRep = int.Parse(values[0]);
                TotX = decimal.Parse(values[1], CultureInfo.InvariantCulture);
                TotNegX = decimal.Parse(values[2], CultureInfo.InvariantCulture);
            }
        }
    }

    public class ReadDateTimeResponse : FiscalResponse
    {
        /// <summary>
        /// Current Date and time in ECR
        /// </summary>
        public DateTime DateTime { get; set; }

        public ReadDateTimeResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                DateTime = DateTime.ParseExact(values[0], "dd-MM-yy HH:mm:ss", CultureInfo.InvariantCulture);
            }
        }
    }

    public class ReadErrorResponse : FiscalResponse
    {
        /// <summary>
        /// Code of the error, to be explained; 
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Explanation of the error Code; 
        /// </summary>
        public string ErrorMessage { get; set; }

        public ReadErrorResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                Code = values[0];
                ErrorMessage = values[1];
            }
        }
    }

    public class ReadStatusResponse : FiscalResponse
    {
        public string[] Status { get; set; }

        public ReadStatusResponse(byte[] buffer)
            : base(buffer)
        {
            if (base.CommandPassed)
            {
                List<string> list = new List<string>();
                for (int index = 0; index < 6; index++)
                {
                    list.Add(getStatusValue(base.Data[index], index));
                }
                list.RemoveAll((string x) => x == string.Empty);
                Status = list.ToArray();
            }
        }

        private string getStatusValue(byte statusByte, int byteIndex)
        {
            switch (byteIndex)
            {
                case 0:
                    if ((statusByte & 0x20) > 0)
                    {
                        return "General error - this is OR of all errors marked with #";
                    }
                    if ((statusByte & 2) > 0)
                    {
                        return "# Command code is invalid.";
                    }
                    if ((statusByte & 1) > 0)
                    {
                        return "# Syntax error.";
                    }
                    break;
                case 1:
                    if ((statusByte & 2) > 0)
                    {
                        return "# Command is not permitted.";
                    }
                    if ((statusByte & 1) > 0)
                    {
                        return "# Overflow during command execution.";
                    }
                    break;
                case 2:
                    if ((statusByte & 0x20) > 0)
                    {
                        return "Nonfiscal receipt is open.";
                    }
                    if ((statusByte & 0x10) > 0)
                    {
                        return "EJ nearly full.";
                    }
                    if ((statusByte & 8) > 0)
                    {
                        return "Fiscal receipt is open.";
                    }
                    if ((statusByte & 4) > 0)
                    {
                        return "EJ is full.";
                    }
                    if ((statusByte & 1) > 0)
                    {
                        return "# End of paper.";
                    }
                    break;
                case 4:
                    if ((statusByte & 0x20) > 0)
                    {
                        return " OR of all errors marked with ‘*’ from Bytes 4 and 5.";
                    }
                    if ((statusByte & 0x10) > 0)
                    {
                        return "* Fiscal memory is full.";
                    }
                    if ((statusByte & 8) > 0)
                    {
                        return "There is space for less then 50 reports in Fiscal memory.";
                    }
                    if ((statusByte & 4) > 0)
                    {
                        return "FM module doesn't exist.";
                    }
                    if ((statusByte & 2) > 0)
                    {
                        return "Tax number is set.";
                    }
                    if ((statusByte & 1) > 0)
                    {
                        return "* Error while writing in FM.";
                    }
                    return string.Empty;
                case 5:
                    if ((statusByte & 0x10) > 0)
                    {
                        return "VAT are set at least once.";
                    }
                    if ((statusByte & 8) > 0)
                    {
                        return "ECR is fiscalized.";
                    }
                    if ((statusByte & 2) > 0)
                    {
                        return "FM is formated.";
                    }
                    break;
                default:
                    return string.Empty;
                case 3:
                    break;
            }
            return string.Empty;
        }
    }

    public class RegisterSaleResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt
        /// </summary>
        public int SlipNumber { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public RegisterSaleResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                SlipNumber = int.Parse(values[0]);
                DocNumber = int.Parse(values[1]);
            }
        }
    }

    public class VoidOpenFiscalReceiptResponse : FiscalResponse
    {
        /// <summary>
        /// Current slip number - unique number of the fiscal receipt
        /// </summary>
        public int SlipNumber { get; set; }

        /// <summary>
        /// Global number of all documents 
        /// </summary>
        public int DocNumber { get; set; }

        public VoidOpenFiscalReceiptResponse(byte[] buffer)
            : base(buffer)
        {
            string[] values = GetDataValues();
            if (values.Length != 0)
            {
                SlipNumber = int.Parse(values[0]);
                DocNumber = int.Parse(values[1]);
            }
        }
    }
    #endregion

    #region Framework
    public class FiscalException : IOException
    {
        public FiscalException(string message)
            : base(message)
        {
        }
    }

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
                ErrorCode = Extensions.ToString(dataBytes);
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
                return Extensions.ToString(Data).Split('\t');
            }
            return new string[0];
        }
    }

    public interface IWrappedMessage
    {
        int Command { get; }

        string Data { get; }

        byte[] GetBytes(int sequence);
    }

    public interface IMessageAggregator
    {
        IObservable<T> GetStream<T>();
        void Publish<T>(T payload);
    }

    public class MessageAggregator : IMessageAggregator
    {
        private readonly object _observablesByTypeKeyLock = new object();

        private readonly Dictionary<string, Tuple<object, object>> _observablesByTypeKey = new Dictionary<string, Tuple<object, object>>();

        public IObservable<T> GetStream<T>()
        {
            string key = typeof(T).ToString();
            IObservable<T> stream;
            lock (_observablesByTypeKeyLock)
            {
                if (_observablesByTypeKey.ContainsKey(key))
                {
                    stream = (IObservable<T>)_observablesByTypeKey[key].Item2;
                }
                else
                {
                    Subject<T> obj = (Subject<T>)Activator.CreateInstance(typeof(Subject<>).MakeGenericType(typeof(T)), new object[0]);
                    stream = obj.Publish().RefCount();
                    Tuple<object, object> tuple = new Tuple<object, object>(obj, stream);
                    _observablesByTypeKey.Add(key, tuple);
                }
            }
            return stream;
        }

        public void Publish<T>(T payload)
        {
            string key = typeof(T).ToString();
            Tuple<object, object> tuple;
            lock (_observablesByTypeKeyLock)
            {
                _observablesByTypeKey.TryGetValue(key, out tuple);
            }
            if (tuple != null)
            {
                ((Subject<T>)tuple.Item1).OnNext(payload);
            }
        }
    }

    public abstract class WrappedMessage : IWrappedMessage
    {
        private readonly Dictionary<char, char> _geo2Rus = new Dictionary<char, char>
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
                throw new InvalidDataException("Packet lenght exceeds the limits.");
            }
            int i = 0;
            string dataConverted = ToANSI(Data);
            int len = dataConverted.Length + 10;
            byte[] packet = new byte[len + 6];
            packet.SetValue((byte)1, i++);
            i = ToQuart(packet, i, len + 32);
            packet.SetValue((byte)sequence, i++);
            i = ToQuart(packet, i, Command);
            i = addData(packet, i, dataConverted);
            packet.SetValue((byte)5, i++);
            i = ToQuart(packet, i, getChecksum(packet));
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

        private static int ToQuart(byte[] buffer, int offset, int value)
        {
            int[] array = new int[4] { 12, 8, 4, 0 };
            foreach (int shifter in array)
            {
                buffer.SetValue((byte)(((value >> shifter) & 0xF) + 48), offset++);
            }
            return offset;
        }

        private string ToANSI(string source)
        {
            string result = string.Empty;
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                result = ((!_geo2Rus.ContainsKey(c)) ? (result + c) : (result + _geo2Rus[c]));
            }
            return result;
        }
    }

    internal static class Extensions
    {
        private static readonly NumberFormatInfo _numFormatInfo;

        static Extensions()
        {
            _numFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
        }

        internal static string Merge(this IEnumerable<object> enumerable, string separator)
        {
            return string.Join("", enumerable.Select(delegate (object x)
            {
                string text = ((!(x.GetType() == typeof(decimal))) ? x.ToString() : ((decimal)x).ToString(_numFormatInfo));
                return text + separator;
            }).ToArray());
        }

        public static string ToString(this byte[] buffer)
        {
            return Encoding.GetEncoding(1251).GetString(buffer);
        }
    }
    #endregion

    #region Events
    public class EcrRespondedEvent
    {
        public IFiscalResponse Response;

        public EcrRespondedEvent(IFiscalResponse response)
        {
            Response = response;
        }
    }

    public class EcrThrewExceptionEvent
    {
        public Exception Exception;

        public EcrThrewExceptionEvent(Exception ex)
        {
            Exception = ex;
        }
    }
    #endregion

    #region Core
    public class FP700 : IDisposable
    {
        private SerialPort _port;

        private int _sequence = 32;

        private bool _innerReadStatusExecuted;

        private readonly Queue<byte> _queue;
        private readonly bool _isFiscal;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="portName"></param>
        public FP700(string portName)
        {
            _isFiscal = false;
            _queue = new Queue<byte>();
            _port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            _port.Open();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_port != null)
            {
                if (_port.IsOpen)
                {
                    _port.Close();
                }
                _port.Dispose();
                _port = null;
            }
        }

        private bool ReadByte()
        {
            int b = _port.ReadByte();
            _queue.Enqueue((byte)b);
            return b != 3;
        }

        private IFiscalResponse SendMessage(IWrappedMessage msg, Func<byte[], IFiscalResponse> responseFactory)
        {
            if (_innerReadStatusExecuted)
            {
                return _SendMessage(msg, responseFactory);
            }
            _SendMessage(new ReadStatusCommand(), (byte[] bytes) => null);
            _innerReadStatusExecuted = true;
            return _SendMessage(msg, responseFactory);
        }

        private IFiscalResponse _SendMessage(IWrappedMessage msg, Func<byte[], IFiscalResponse> responseFactory)
        {
            IFiscalResponse response = null;
            byte[] lastStatusBytes = null;
            byte[] packet = msg.GetBytes(_sequence);
            for (int r = 0; r < 3; r++)
            {
                try
                {
                    _port.Write(packet, 0, packet.Length);
                    List<byte> list = new List<byte>();
                    while (ReadByte())
                    {
                        byte b = _queue.Dequeue();
                        switch (b)
                        {
                            case 21:
                                throw new IOException("Invalid packet checksum or form of messsage.");
                            case 22:
                                continue;
                        }
                        list.Add(b);
                    }
                    list.Add(_queue.Dequeue());
                    byte[] buffer = list.ToArray();
                    response = responseFactory(buffer);
                    lastStatusBytes = list.Skip(list.IndexOf(4) + 1).Take(6).ToArray();
                }
                catch (Exception)
                {
                    if (r >= 2)
                    {
                        throw;
                    }
                    _queue.Clear();
                    continue;
                }
                break;
            }
            _sequence++;
            if (_sequence > 254)
            {
                _sequence = 32;
            }
            if (msg.Command != 74)
            {
                CheckStatusOnErrors(lastStatusBytes);
            }
            return response;
        }

        private void CheckStatusOnErrors(byte[] statusBytes)
        {
            if (statusBytes == null)
            {
                throw new ArgumentNullException("statusBytes");
            }
            if (statusBytes.Length == 0)
            {
                throw new ArgumentException("Argument is empty collection", "statusBytes");
            }
            if ((statusBytes[0] & 0x20) > 0)
            {
                throw new FiscalException("General error - this is OR of all errors marked with #");
            }
            if ((statusBytes[0] & 2) > 0)
            {
                throw new FiscalException("# Command code is invalid.");
            }
            if ((statusBytes[0] & 1) > 0)
            {
                throw new FiscalException("# Syntax error.");
            }
            if ((statusBytes[1] & 2) > 0)
            {
                throw new FiscalException("# Command is not permitted.");
            }
            if ((statusBytes[1] & 1) > 0)
            {
                throw new FiscalException("# Overflow during command execution.");
            }
            if ((statusBytes[2] & 1) > 0)
            {
                throw new FiscalException("# End of paper.");
            }
            if ((statusBytes[4] & 0x20) > 0)
            {
                throw new FiscalException(" OR of all errors marked with ‘*’ from Bytes 4 and 5.");
            }
            if ((statusBytes[4] & 0x10) > 0)
            {
                throw new FiscalException("* Fiscal memory is full.");
            }
            if ((statusBytes[4] & 1) > 0)
            {
                throw new FiscalException("* Error while writing in FM.");
            }
        }

        /// <summary> 
        /// Changes port name at runtime. 
        /// </summary> 
        /// <param name="portName">Name of the serial port.</param> 
        public void ChangePort(string portName)
        {
            _port.Close();
            _port.PortName = portName;
            _port.Open();
        }

        /// <summary>
        /// Executes custom command implementation and returns predefined custom response.
        /// </summary>
        /// <typeparam name="T">Response Type. Must be a child of an abstract class FiscalResponse</typeparam>
        /// <param name="cmd">Command to execute. Must be a child of an abstract class WrappedMessage</param>
        /// <returns>T</returns>
        public T ExecuteCustomCommand<T>(WrappedMessage cmd) where T : FiscalResponse
        {
            return (T)SendMessage(cmd, (byte[] bytes) => (FiscalResponse)Activator.CreateInstance(typeof(T), bytes));
        }

        /// <summary>
        /// Opens non fiscal text receipt.
        /// </summary>
        /// <returns>CommonFiscalResponse</returns>
        public CommonFiscalResponse OpenNonFiscalReceipt()
        {
            return (CommonFiscalResponse)SendMessage(new OpenNonFiscalReceiptCommand(), (byte[] bytes) => new CommonFiscalResponse(bytes));
        }

        /// <summary>
        /// Printing of free text in a non-fiscal text receipt
        /// </summary>
        /// <param name="text">Up to 30 symbols.</param>
        /// <returns></returns>
        public CommonFiscalResponse AddTextToNonFiscalReceipt(string text)
        {
            return (CommonFiscalResponse)SendMessage(new AddTextToNonFiscalReceiptCommand(text), (byte[] bytes) => new CommonFiscalResponse(bytes));
        }

        /// <summary>
        /// Closes non fiscal text receipt.
        /// </summary>
        /// <returns>CommonFiscalResponse</returns>
        public CommonFiscalResponse CloseNonFiscalReceipt()
        {
            return (CommonFiscalResponse)SendMessage(new CloseNonFiscalReceiptCommand(), (byte[] bytes) => new CommonFiscalResponse(bytes));
        }

        /// <summary>
        /// Opens Sales Fiscal Receipt
        /// </summary>
        /// <param name="opCode">Operator code</param>
        /// <param name="opPwd">Operator password</param>
        /// <returns>OpenFiscalReceiptResponse</returns>
        public OpenFiscalReceiptResponse OpenFiscalReceipt(string opCode, string opPwd)
        {
            return (OpenFiscalReceiptResponse)SendMessage(new OpenFiscalReceiptCommand(opCode, opPwd), (byte[] bytes) => new OpenFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Opens Fiscal Receipt
        /// </summary>
        /// <param name="opCode">Operator code</param>
        /// <param name="opPwd">Operator password</param>
        /// <param name="type">Receipt type</param>
        /// <returns>OpenFiscalReceiptResponse</returns>
        public OpenFiscalReceiptResponse OpenFiscalReceipt(string opCode, string opPwd, ReceiptType type)
        {
            return (OpenFiscalReceiptResponse)SendMessage(new OpenFiscalReceiptCommand(opCode, opPwd, (int)type), (byte[] bytes) => new OpenFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Opens Fiscal Receipt
        /// </summary>
        /// <param name="opCode">Operator code</param>
        /// <param name="opPwd">Operator password</param>
        /// <param name="type">Receipt type</param>
        /// <param name="tillNumber">Number of point of sale (1...999). Default: Logical number of the ECR in the workplace; </param>
        /// <returns>OpenFiscalReceiptResponse</returns>
        public OpenFiscalReceiptResponse OpenFiscalReceipt(string opCode, string opPwd, ReceiptType type, int tillNumber)
        {
            return (OpenFiscalReceiptResponse)SendMessage(new OpenFiscalReceiptCommand(opCode, opPwd, (int)type, tillNumber), (byte[] bytes) => new OpenFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluName">Item name (up to 32 symbols)</param>
        /// <param name="price">Product price. With sign '-' at void operations;</param>
        /// <param name="departmentNumber">Between 1 and 16.</param>
        /// <param name="quantity"> Quantity. NOTE: Max value: {Quantity} * {Price} is 9999999.99</param>
        /// <param name="taxCode">Optional Parameter. Tax code: 1-A, 2-B, 3-C; default = TaxCode.A</param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterSale(string pluName, decimal price, decimal quantity, int departmentNumber, TaxCode taxCode = TaxCode.A)
        {
            return (RegisterSaleResponse)SendMessage(new RegisterSaleCommand(pluName, (int)taxCode, price, departmentNumber, quantity), (byte[] bytes) => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluName">Item name (up to 32 symbols)</param>
        /// <param name="price">Product price. With sign '-' at void operations;</param>
        /// <param name="departmentNumber">Between 1 and 16.</param>
        /// <param name="quantity"> Quantity. NOTE: Max value: {Quantity} * {Price} is 9999999.99</param>
        /// <param name="discountType">Type of the discount.</param>
        /// <param name="discountValue">Discount Value. Percentage ( 0.00 - 100.00 ) for percentage operations; Amount ( 0.00 - 9999999.99 ) for value operations; Note: If {DiscountType} is given, {DiscountValue} must contain value. </param>
        /// <param name="taxCode">Optional Parameter. Tax code: 1-A, 2-B, 3-C; default = TaxCode.A</param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterSale(string pluName, decimal price, decimal quantity, int departmentNumber, DiscountType discountType, decimal discountValue, TaxCode taxCode = TaxCode.A)
        {
            return (RegisterSaleResponse)SendMessage(new RegisterSaleCommand(pluName, (int)taxCode, price, departmentNumber, quantity, (int)discountType, discountValue), (byte[] bytes) => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluCode">The code of the item (1 - 100000). With sign '-' at void operations; </param>
        /// <param name="price"> Price of the item (0.01 - 9999999.99). Default: programmed price of the item; </param>
        /// <param name="quantity"> Quantity of the item (0.001 - 99999.999) </param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterProgrammedItemSale(int pluCode, decimal price, decimal quantity)
        {
            return (RegisterSaleResponse)SendMessage(new RegisterProgrammedItemSaleCommand(pluCode, price, quantity), (byte[] bytes) => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Adds new Item to open receipt
        /// </summary>
        /// <param name="pluCode">The code of the item (1 - 100000). With sign '-' at void operations; </param>
        /// <param name="price"> Price of the item (0.01 - 9999999.99). Default: programmed price of the item; </param>
        /// <param name="quantity"> Quantity of the item (0.001 - 99999.999) </param>
        /// <param name="discountType">Type of the discount.</param>
        /// <param name="discountValue">Discount Value. Percentage ( 0.00 - 100.00 ) for percentage operations; Amount ( 0.00 - 9999999.99 ) for value operations; Note: If {DiscountType} is given, {DiscountValue} must contain value. </param>
        /// <returns>RegisterSaleResponse</returns>
        public RegisterSaleResponse RegisterProgrammedItemSale(int pluCode, decimal price, decimal quantity, DiscountType discountType, decimal discountValue)
        {
            return (RegisterSaleResponse)SendMessage(new RegisterProgrammedItemSaleCommand(pluCode, price, quantity, (int)discountType, discountValue), (byte[] bytes) => new RegisterSaleResponse(bytes));
        }

        /// <summary>
        /// Payments and calculation of the total sum
        /// </summary>
        /// <param name="paymentMode"> Type of payment. Default: 'Cash' </param>
        /// <returns>CalculateTotalResponse</returns>
        public CalculateTotalResponse Total(PaymentMode paymentMode = PaymentMode.Cash)
        {
            return (CalculateTotalResponse)SendMessage(new CalculateTotalCommand((int)paymentMode), (byte[] bytes) => new CalculateTotalResponse(bytes));
        }

        /// <summary>
        /// All void of a fiscal receipt. <br />
        /// <bold>Note:The receipt will be closed as a non fiscal receipt. The slip number (unique number of the fiscal receipt) will not be increased.</bold>
        /// </summary>
        /// <returns>VoidOpenFiscalReceiptResponse</returns>
        public VoidOpenFiscalReceiptResponse VoidOpenFiscalReceipt()
        {
            return (VoidOpenFiscalReceiptResponse)SendMessage(new VoidOpenFiscalReceiptCommand(), (byte[] bytes) => new VoidOpenFiscalReceiptResponse(bytes));
        }

        public AddTextToFiscalReceiptResponse AddTextToFiscalReceipt(string text)
        {
            return (AddTextToFiscalReceiptResponse)SendMessage(new AddTextCommand(text, _isFiscal), (byte[] bytes) => new AddTextToFiscalReceiptResponse(bytes));
        }

        /// <summary>
        ///  Closes open fiscal receipt.
        /// </summary>
        /// <returns>CloseFiscalReceiptResponse</returns>
        public CloseFiscalReceiptResponse CloseFiscalReceipt()
        {
            return (CloseFiscalReceiptResponse)SendMessage(new CloseFiscalReceiptCommand(), (byte[] bytes) => new CloseFiscalReceiptResponse(bytes));
        }

        /// <summary>
        /// Get the information on the last fiscal entry.
        /// </summary>
        /// <param name="type">FiscalEntryInfoType. Default: FiscalEntryInfoType.CashDebit</param>
        /// <returns>GetLastFiscalEntryInfoResponse</returns>
        public GetLastFiscalEntryInfoResponse GetLastFiscalEntryInfo(FiscalEntryInfoType type = FiscalEntryInfoType.CashDebit)
        {
            return (GetLastFiscalEntryInfoResponse)SendMessage(new GetLastFiscalEntryInfoCommand((int)type), (byte[] bytes) => new GetLastFiscalEntryInfoResponse(bytes));
        }

        /// <summary>
        /// Cash in and Cash out operations
        /// </summary>
        /// <param name="operationType">Type of operation</param>
        /// <param name="amount">The sum</param>
        /// <returns>CashInCashOutResponse</returns>
        public CashInCashOutResponse CashInCashOutOperation(Cash operationType, decimal amount)
        {
            return (CashInCashOutResponse)SendMessage(new CashInCashOutCommand((int)operationType, amount), (byte[] bytes) => new CashInCashOutResponse(bytes));
        }

        /// <summary>
        /// Reads the status of the device.
        /// </summary>
        /// <returns>ReadStatusResponse</returns>
        public ReadStatusResponse ReadStatus()
        {
            return (ReadStatusResponse)SendMessage(new ReadStatusCommand(), (byte[] bytes) => new ReadStatusResponse(bytes));
        }

        /// <summary>
        /// Feeds blank paper.
        /// </summary>
        /// <param name="lines">Line Count 1 to 99; default =  1;</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse FeedPaper(int lines = 1)
        {
            return (EmptyFiscalResponse)SendMessage(new FeedPaperCommand(lines), (byte[] bytes) => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Prints buffer
        /// </summary>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse PrintBuffer()
        {
            return (EmptyFiscalResponse)SendMessage(new FeedPaperCommand(0), (byte[] bytes) => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Reads an error code  explanation from ECR.
        /// </summary>
        /// <param name="errorCode">Code of the error</param>
        /// <returns>ReadErrorResponse</returns>
        public ReadErrorResponse ReadError(string errorCode)
        {
            return (ReadErrorResponse)SendMessage(new ReadErrorCommand(errorCode), (byte[] bytes) => new ReadErrorResponse(bytes));
        }

        /// <summary>
        /// ECR beeps with given interval and frequency.
        /// </summary>
        /// <param name="frequency">in hertzes</param>
        /// <param name="interval">in milliseconds</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse PlaySound(int frequency, int interval)
        {
            return (EmptyFiscalResponse)SendMessage(new PlaySoundCommand(frequency, interval), (byte[] bytes) => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Prints X or Z Report and returns some stats.
        /// </summary>
        /// <param name="type">ReportType</param>
        /// <returns>PrintReportResponse</returns>
        public PrintReportResponse PrintReport(ReportType type)
        {
            return (PrintReportResponse)SendMessage(new PrintReportCommand(type.ToString()), (byte[] bytes) => new PrintReportResponse(bytes));
        }

        /// <summary>
        /// Opens the cash drawer if such is connected.
        /// </summary>
        /// <param name="impulseLength"> The length of the impulse in milliseconds. </param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse OpenDrawer(int impulseLength)
        {
            return (EmptyFiscalResponse)SendMessage(new OpenDrawerCommand(impulseLength), (byte[] bytes) => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Sets date and time in ECR.
        /// </summary>
        /// <param name="dateTime">DateTime to set.</param>
        /// <returns>EmptyFiscalResponse</returns>
        public EmptyFiscalResponse SetDateTime(DateTime dateTime)
        {
            return (EmptyFiscalResponse)SendMessage(new SetDateTimeCommand(dateTime), (byte[] bytes) => new EmptyFiscalResponse(bytes));
        }

        /// <summary>
        /// Reads current date and time from ECR.
        /// </summary>
        /// <returns>ReadDateTimeResponse</returns>
        public ReadDateTimeResponse ReadDateTime()
        {
            return (ReadDateTimeResponse)SendMessage(new ReadDateTimeCommand(), (byte[] bytes) => new ReadDateTimeResponse(bytes));
        }

        /// <summary>
        /// Gets the status of current or last receipt 
        /// </summary>
        /// <returns>GetStatusOfCurrentReceiptResponse</returns>
        public GetStatusOfCurrentReceiptResponse GetStatusOfCurrentReceipt()
        {
            return (GetStatusOfCurrentReceiptResponse)SendMessage(new GetStatusOfCurrentReceiptCommand(), (byte[] bytes) => new GetStatusOfCurrentReceiptResponse(bytes));
        }

        /// <summary>
        /// Defines items in ECR
        /// </summary>
        /// <param name="name"></param>
        /// <param name="plu"></param>
        /// <param name="taxGr"></param>
        /// <param name="dep"></param>
        /// <param name="group"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <param name="priceType"></param>
        /// <returns></returns>
        public EmptyFiscalResponse ProgramItem(string name, int plu, TaxGr taxGr, int dep, int group, decimal price, decimal quantity = 9999m, PriceType priceType = PriceType.FixedPrice)
        {
            return (EmptyFiscalResponse)SendMessage(new ProgramItemCommand(name, plu, taxGr, dep, group, price, quantity, priceType), (byte[] bytes) => new EmptyFiscalResponse(bytes));
        }
    }
    #endregion
}
