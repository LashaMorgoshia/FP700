using System.IO.Ports;
using System.Text;
using System;
using System.Security.Cryptography;

namespace FP700
{
    public class Program
    {
        public static void Main()
        {
            UDatecsProtocol.USBLib usb = new UDatecsProtocol.USBLib("COM3", 115200);
            usb.Open();
            string command = BuildCommand("26h", "");
            byte[] message = BuildWrappedMessage(command);
            usb.Write(message, 0, message.Length);
            usb.Close();


            //var printer = new FiscalPrinterTest();

            //// Connect to the fiscal printer on COM3 at 9600 baud rate
            //printer.Connect("COM3", 115200);

            //// Open non-fiscal receipt
            //printer.OpenNonFiscalReceipt();

            //// Print non-fiscal text
            //printer.PrintNonFiscalText("Test Non-Fiscal Mode");

            //// Close non-fiscal receipt
            //printer.CloseNonFiscalReceipt();

            //// Close the connection
            //printer.Disconnect();
        }

        // FOR PRODUCTION USE ONLY
        //public static void Main()
        //{
        //    FiscalPrinter printer = new FiscalPrinter();

        //    // Connect to the fiscal printer on COM3 at 9600 baud rate
        //    printer.Connect("COM3");

        //    // Initialize fiscal receipt
        //    printer.InitFiscalReceipt("1", "0000", "003");

        //    // Print non-fiscal text
        //    printer.PrintNonFiscalText("Hello World!");

        //    // Close the connection
        //    printer.Disconnect();
        //}

        // Method to send the command to the printer
        private static byte[] BuildWrappedMessage(string command)
        {
            // Example wrapping: preamble (01h), command, terminator (03h)
            string wrappedCommand = $"01{command}03";  // Customize wrapping as needed
            return Encoding.ASCII.GetBytes(wrappedCommand);
        }

        private static string BuildCommand(string commandCode, string parameters)
        {
            return $"{commandCode}\t{parameters}";
        }

    }
}
