using System;
using System.IO.Ports;
using System.Text;

namespace PD25
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FiscalPrinter printer = new FiscalPrinter("COM3", 115200);

            try
            {
                // Open the connection to the printer
                printer.Open();

                // Open a fiscal receipt
                //printer.OpenFiscalReceipt(1, "0000", 1); // Operator code, Operator password, Till number
                printer.OpenFiscalReceipt("001", "0001", "001"); // Operator code "001", password "1234", till number "001"
                System.Threading.Thread.Sleep(1000); // Wait for the printer to process

                // Register an item: Code, Price, Qty, Sum
                printer.RegisterItem("Item 001", 1, 12.50, 2, 1); // Item Name, TaxCode, Price, Quantity, Department
                System.Threading.Thread.Sleep(1000); // Wait for the printer to process

                printer.RegisterItem("Item 002", 1, 5.00, 1, 1); // Another item
                System.Threading.Thread.Sleep(1000); // Wait for the printer to process

                // Close the fiscal receipt
                printer.CloseFiscalReceipt();
                System.Threading.Thread.Sleep(1000); // Wait for the printer to process
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                // Ensure the port is closed at the end
                printer.Close();
            }
        }
    }

    class FiscalPrinter
    {
        private SerialPort _serialPort;

        public FiscalPrinter(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        // Open serial port connection
        public void Open()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
                Console.WriteLine("Serial port opened.");
            }
        }

        // Close serial port connection
        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                Console.WriteLine("Serial port closed.");
            }
        }

        // Send data to the fiscal printer
        public void SendCommand(byte[] command)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(command, 0, command.Length);
                Console.WriteLine("Command sent to the printer.");
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }

        // Handle data received from the fiscal printer
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine("Data Received:");
            Console.WriteLine(indata);
        }

        // Command to open a fiscal receipt (Command 48)
        public void OpenFiscalReceipt(int operatorCode, string operatorPassword, int tillNumber)
        {
            byte[] command = new byte[]
            {
                0x01, 0x20, 0x30, 0x48,
                (byte)(operatorCode + 0x30),
                (byte)(operatorPassword[0]),
                (byte)(tillNumber + 0x30),
                0x30, 0x05, 0x30, 0x30, 0x03
            };
            SendCommand(command);
        }

        public void OpenFiscalReceipt(string operatorCode, string operatorPassword, string tillNumber)
        {
            // Convert the operatorCode, operatorPassword, and tillNumber into ASCII bytes
            byte[] operatorCodeBytes = Encoding.ASCII.GetBytes(operatorCode); // Convert "001" to ASCII
            byte[] operatorPasswordBytes = Encoding.ASCII.GetBytes(operatorPassword); // Convert password (e.g., "1234") to ASCII
            byte[] tillNumberBytes = Encoding.ASCII.GetBytes(tillNumber); // Convert till number (e.g., "001") to ASCII

            // Construct the command using the ASCII byte arrays
            byte[] command = new byte[]
            {
        0x01, 0x20, 0x30, 0x48,  // Preamble and Command 48 (Open Fiscal Receipt)

        // Operator code in ASCII (3 bytes if "001", for example)
        operatorCodeBytes[0], operatorCodeBytes[1], operatorCodeBytes[2], 

        // Operator password in ASCII (first character, can be multiple depending on password length)
        operatorPasswordBytes[0], operatorPasswordBytes[1], operatorPasswordBytes[2], operatorPasswordBytes[3],

        // Till number in ASCII (could be a 3-digit number like "001")
        tillNumberBytes[0], tillNumberBytes[1], tillNumberBytes[2],

        // Postamble and Terminator
        0x05, 0x30, 0x30, 0x03
            };

            // Send the command
            SendCommand(command);
        }


        // Command to register an item (Command 49)
        public void RegisterItem(string itemName, int taxCode, double price, double quantity, int department)
        {
            string priceStr = price.ToString("F2").Replace(".", "").PadLeft(9, '0'); // Format price: 000000000
            string qtyStr = quantity.ToString("F3").Replace(".", "").PadLeft(9, '0'); // Format quantity: 000000000

            byte[] command = new byte[]
            {
            0x01, // Preamble
            0x20, // Length (example)
            0x30, // Sequence number
            0x49, // Command 49 (Registration of sale)
            (byte)taxCode, // TaxCode
            Encoding.ASCII.GetBytes(priceStr)[0], // Price (formatted as ASCII bytes)
            Encoding.ASCII.GetBytes(qtyStr)[0],   // Quantity (formatted as ASCII bytes)
            (byte)department, // Department
            0x05, // Postamble
            0x03  // Terminator
            };

            SendCommand(command);
        }

        // Command to close fiscal receipt (Command 56)
        public void CloseFiscalReceipt()
        {
            byte[] closeReceiptCommand = new byte[] { 0x01, 0x20, 0x30, 0x56, 0x05, 0x30, 0x30, 0x30, 0x30, 0x03 };
            SendCommand(closeReceiptCommand);
        }

        // Command to print free text in fiscal receipt (optional for item description)
        public void PrintFreeText(string text)
        {
            byte[] textBytes = Encoding.ASCII.GetBytes(text);
            byte[] command = new byte[textBytes.Length + 6];

            command[0] = 0x01; // Preamble
            command[1] = 0x20; // Length (example)
            command[2] = 0x30; // Sequence number
            command[3] = 0x54; // Command 54 (Print free text)
            Array.Copy(textBytes, 0, command, 4, textBytes.Length); // Copy the text into the command
            command[textBytes.Length + 4] = 0x05; // Postamble
            command[textBytes.Length + 5] = 0x03; // Terminator

            SendCommand(command);
        }
    }
}
