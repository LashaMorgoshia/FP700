using System;
using System.IO.Ports;

namespace FP700
{
    public class DatecsFiscalPrinter
    {
        private SerialPort _serialPort;

        public DatecsFiscalPrinter(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
            {
                ReadTimeout = 500,
                WriteTimeout = 500
            };
        }

        public void Open()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        public void Close()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void OpenFiscalReceipt(int operatorId, string operatorPassword, int tillNumber)
        {
            // Command to open fiscal receipt - Command 30H (48)
            string command = $"{(char)0x01}{(char)0x20}{(char)operatorId},{operatorPassword},{tillNumber}{(char)0x05}{(char)0x03}";
            SendCommand(command);
        }

        public void PrintSale(string itemName, decimal price, int quantity, char taxGroup)
        {
            // Command to enter a sale - Command 31H (49)
            string command = $"{itemName}\t{taxGroup}{price:0.00}*{quantity}{(char)0x05}{(char)0x03}";
            SendCommand(command);
        }

        public void CloseFiscalReceipt()
        {
            // Command to close fiscal receipt - Command 38H (56)
            string command = $"{(char)0x01}{(char)0x38}{(char)0x05}{(char)0x03}";
            SendCommand(command);
        }

        private void SendCommand(string command)
        {
            if (_serialPort.IsOpen)
            {
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(command);
                _serialPort.Write(buffer, 0, buffer.Length);

                // Read response
                byte[] responseBuffer = new byte[1024];
                int bytesRead = _serialPort.Read(responseBuffer, 0, responseBuffer.Length);
                string response = System.Text.Encoding.ASCII.GetString(responseBuffer, 0, bytesRead);
                Console.WriteLine("Response: " + response);
            }
            else
            {
                throw new InvalidOperationException("Serial port is not open.");
            }
        }
    }
}