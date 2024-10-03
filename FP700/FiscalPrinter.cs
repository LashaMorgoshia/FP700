using System;
using System.IO.Ports;
using System.Text;

namespace FP700
{
    public class FiscalPrinter
    {
        private SerialPort _serialPort;

        // Connect to the fiscal printer
        public void Connect(string portName, int baudRate = 9600)
        {
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                Encoding = Encoding.GetEncoding("ISO-8859-1"), // Set appropriate encoding if needed
                ReadTimeout = 500,  // 500ms timeout
                WriteTimeout = 500   // 500ms timeout
            };

            try
            {
                _serialPort.Open();
                Console.WriteLine("Connected to fiscal printer.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to fiscal printer: {ex.Message}");
            }
        }

        // Initialize printer (Example Command 48 - Open Fiscal Receipt)
        public void InitFiscalReceipt(string operatorNumber, string operatorPassword, string tillNumber)
        {
            string command = BuildCommand("30h", $"{operatorNumber}\t{operatorPassword}\t{tillNumber}\t");
            SendCommand(command);
        }

        // Example command to print a free non-fiscal text (Command 42)
        public void PrintNonFiscalText(string text)
        {
            string command = BuildCommand("2Ah", $"{text}\t");
            SendCommand(command);
        }

        // Method to send the command to the printer
        private void SendCommand(string command)
        {
            if (_serialPort.IsOpen)
            {
                byte[] message = BuildWrappedMessage(command);
                _serialPort.Write(message, 0, message.Length);

                // Read the response from the printer
                byte[] buffer = new byte[256];
                int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Response from printer: {response}");
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }

        // Build the wrapped message format as per protocol
        private byte[] BuildWrappedMessage(string command)
        {
            // Add necessary wrapping here based on protocol, such as checksum and preamble
            string wrappedCommand = $"01{command}03";  // Example format: preamble (01h), command, terminator (03h)
            return Encoding.ASCII.GetBytes(wrappedCommand);
        }

        // Helper method to build commands
        private string BuildCommand(string commandCode, string parameters)
        {
            // Command format: <CMD><TAB><PARAM1><TAB><PARAM2>...
            return $"{commandCode}\t{parameters}";
        }

        // Disconnect the serial port
        public void Disconnect()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                Console.WriteLine("Disconnected from fiscal printer.");
            }
        }
    }

    public class FiscalPrinterTest
    {
        private SerialPort _serialPort;

        // Connect to the fiscal printer
        public void Connect(string portName, int baudRate = 9600)
        {
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                Encoding = Encoding.GetEncoding("ISO-8859-1"), // Set appropriate encoding if needed
                ReadTimeout = 12000,  // Increase the timeout for testing
                WriteTimeout = 12000   // Increase the timeout for testing
            };

            try
            {
                _serialPort.Open();
                Console.WriteLine("Connected to fiscal printer.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to fiscal printer: {ex.Message}");
            }
        }

        // Open a non-fiscal receipt (Command 38h)
        public void OpenNonFiscalReceipt()
        {
            string command = BuildCommand("26h", "");  // Open non-fiscal receipt
            SendCommand(command);
        }

        // Print free non-fiscal text (Command 42h)
        public void PrintNonFiscalText(string text)
        {
            string command = BuildCommand("2Ah", $"{text}\t");  // Print text
            SendCommand(command);
        }

        // Close non-fiscal receipt (Command 39h)
        public void CloseNonFiscalReceipt()
        {
            string command = BuildCommand("27h", "");  // Close non-fiscal receipt
            SendCommand(command);
        }

        // Method to send the command to the printer
        private void SendCommand(string command)
        {
            if (_serialPort.IsOpen)
            {
                byte[] message = BuildWrappedMessage(command);
                _serialPort.Write(message, 0, message.Length);

                try
                {
                    // Read the response from the printer
                    byte[] buffer = new byte[256];
                    int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Response from printer: {response}");
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"Timeout occurred: {ex.Message}");
                    // Handle the timeout here or retry
                }
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }

        // Build the wrapped message format as per protocol
        private byte[] BuildWrappedMessage(string command)
        {
            // Example wrapping: preamble (01h), command, terminator (03h)
            string wrappedCommand = $"01{command}03";  // Customize wrapping as needed
            return Encoding.ASCII.GetBytes(wrappedCommand);
        }

        // Helper method to build commands
        private string BuildCommand(string commandCode, string parameters)
        {
            return $"{commandCode}\t{parameters}";
        }

        // Disconnect the serial port
        public void Disconnect()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                Console.WriteLine("Disconnected from fiscal printer.");
            }
        }
    }
}
