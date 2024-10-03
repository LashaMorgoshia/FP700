using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP700
{
    internal class Exp01
    {
        //class USBCommunication
        //{
        //    private SerialPort _serialPort;

        //    public void InitializeConnection()
        //    {
        //        // Create a new SerialPort object with default settings for COM3.
        //        _serialPort = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One);

        //        // Set read/write timeouts
        //        _serialPort.ReadTimeout = 500;
        //        _serialPort.WriteTimeout = 500;

        //        // Open the port for communication
        //        try
        //        {
        //            _serialPort.Open();
        //            Console.WriteLine("Connection opened on COM3.");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error: Unable to open COM3. {ex.Message}");
        //        }
        //    }

        //    public void SendData(string data)
        //    {
        //        if (_serialPort != null && _serialPort.IsOpen)
        //        {
        //            try
        //            {
        //                _serialPort.WriteLine(data);
        //                Console.WriteLine("Data sent successfully.");
        //            }
        //            catch (TimeoutException)
        //            {
        //                Console.WriteLine("Write timeout occurred.");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("COM3 is not open.");
        //        }
        //    }

        //    public void ReceiveData()
        //    {
        //        if (_serialPort != null && _serialPort.IsOpen)
        //        {
        //            try
        //            {
        //                string message = _serialPort.ReadLine();
        //                Console.WriteLine($"Received: {message}");
        //            }
        //            catch (TimeoutException)
        //            {
        //                Console.WriteLine("Read timeout occurred.");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("COM3 is not open.");
        //        }
        //    }

        //    public void CloseConnection()
        //    {
        //        if (_serialPort != null && _serialPort.IsOpen)
        //        {
        //            _serialPort.Close();
        //            Console.WriteLine("Connection closed.");
        //        }
        //    }

        //}

        //public class FiscalPrinter
        //{
        //    private DatecsProtocol protocol;

        //    public void InitializeFiscalPrinter(string portName, uint baudrate)
        //    {
        //        try
        //        {
        //            // Initialize the DatecsProtocol instance
        //            protocol = new DatecsProtocol();

        //            // Set the serial port settings (RS232 connection)
        //            protocol.SetRS232(portName, baudrate); // e.g., "COM3", 115200

        //            // Open the connection to the fiscal device
        //            protocol.OpenConnection();

        //            if (protocol.IsOpen)
        //            {
        //                Console.WriteLine("Connection to fiscal printer opened successfully.");
        //            }
        //            else
        //            {
        //                Console.WriteLine("Failed to open connection to fiscal printer.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error initializing fiscal printer: {ex.Message}");
        //        }
        //    }

        //    public void PrintFiscalReceipt()
        //    {
        //        try
        //        {
        //            string output = string.Empty;
        //            // Example: Sending a print command (assuming command 45 is for printing)
        //            // You may need to adjust the command and parameters based on your device's protocol
        //            protocol.ExecuteCommand(45, "1\t", ref output); // '1\t' may represent receipt print mode

        //            if (protocol.LastError == 0)
        //            {
        //                Console.WriteLine("Fiscal receipt printed successfully.");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Error: {protocol.LastError}. Unable to print receipt.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"An error occurred: {ex.Message}");
        //        }
        //    }

        //    public void TestPrintCommand()
        //    {
        //        string output = string.Empty;

        //        // Example of sending a test print command (use appropriate command ID for your printer)
        //        protocol.ExecuteCommand(100, "1\t", ref output); // Command `100` is just an example, check your protocol

        //        if (protocol.LastError == 0)
        //        {
        //            Console.WriteLine($"Test Print Successful. Response: {output}");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Error: {protocol.LastError}");
        //        }
        //    }

        //    public void ExecuteFiscalOperations()
        //    {
        //        try
        //        {
        //            string output = string.Empty;

        //            // Step 1: Open Fiscal Receipt
        //            protocol.ExecuteCommand(48, "1\t0000\t1\t\t\t", ref output);  // Adjust the parameters based on your printer
        //            if (protocol.LastError != 0)
        //            {
        //                throw new Exception($"Error opening fiscal receipt: {protocol.LastError}");
        //            }

        //            // Step 2: Add an Item to the Receipt
        //            protocol.ExecuteCommand(49, "PLU001\t1\t10.00\t1\t\t\t1\t", ref output);  // Adjust the parameters based on your printer
        //            if (protocol.LastError != 0)
        //            {
        //                throw new Exception($"Error adding item to receipt: {protocol.LastError}");
        //            }

        //            // Step 3: Print Fiscal Receipt
        //            protocol.ExecuteCommand(45, "1\t", ref output);  // Adjust the command based on your printer
        //            if (protocol.LastError != 0)
        //            {
        //                throw new Exception($"Error printing receipt: {protocol.LastError}");
        //            }

        //            Console.WriteLine("Fiscal operations completed successfully.");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }

        //    public void OpenFiscalReceipt()
        //    {
        //        string output = string.Empty;

        //        // Open fiscal receipt (example: operator ID and password might be required)
        //        protocol.ExecuteCommand(48, "1\t0000\t1\t\t\t", ref output);  // Example values, check your printer documentation

        //        if (protocol.LastError == 0)
        //        {
        //            Console.WriteLine("Fiscal receipt opened successfully.");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Error opening fiscal receipt: {protocol.LastError}");
        //        }
        //    }

        //    public void CheckStatus()
        //    {
        //        string output = string.Empty;
        //        protocol.ExecuteCommand(255, "STATUS", ref output);  // Use the correct command for checking printer status
        //        Console.WriteLine(output);

        //        if (protocol.LastError != 0)
        //        {
        //            Console.WriteLine($"Error encountered: {protocol.LastError}");
        //        }
        //        else
        //        {
        //            Console.WriteLine("Command executed successfully.");
        //        }
        //    }

        //    public void CloseConnection()
        //    {
        //        if (protocol != null && protocol.IsOpen)
        //        {
        //            protocol.CloseConnection();
        //            Console.WriteLine("Connection to fiscal printer closed.");
        //        }
        //    }

        //    public bool IsConnectionOpen()
        //    {
        //        return protocol != null && protocol.IsOpen;
        //    }
        //}

        //internal class Program
        //{
        //    //// Dictionary to hold error codes and messages
        //    //static Dictionary<int, string> ErrorMessages = new Dictionary<int, string>();

        //    //static void Main(string[] args)
        //    //{
        //    //    // Load error messages
        //    //    LoadErrorMessages();

        //    //    // Initialize the protocol
        //    //    DatecsProtocol protocol = new DatecsProtocol();

        //    //    // Set up the connection parameters
        //    //    string portName = "COM3"; // Replace with your COM port
        //    //    uint baudRate = 115200;    // Replace with your baud rate

        //    //    protocol.SetRS232(portName, baudRate);

        //    //    try
        //    //    {
        //    //        // Open the connection
        //    //        protocol.OpenConnection();

        //    //        // Check if the connection is open
        //    //        if (protocol.IsOpen)
        //    //        {
        //    //            // Log in with operator ID and password
        //    //            string operatorId = "1";
        //    //            string operatorPassword = "0000";
        //    //            string receiptNumber = "3";

        //    //            string output = "";

        //    //            // Open a fiscal receipt (Command 48)
        //    //            protocol.ExecuteCommand(48, $"{operatorId}\t{operatorPassword}\t{receiptNumber}\t1\t\t", ref output);
        //    //            CheckResult(protocol);

        //    //            // Sell an item (Command 49)
        //    //            string itemName = "Product1";
        //    //            string vatGroup = "1"; // VAT group
        //    //            string price = "10.00";
        //    //            string quantity = "1";

        //    //            protocol.ExecuteCommand(49, $"{itemName}\t{vatGroup}\t{price}\t{quantity}\t\t\t1\t", ref output);
        //    //            CheckResult(protocol);

        //    //            // Process Payment (Command 53)
        //    //            string paymentType = "0"; // Cash
        //    //            protocol.ExecuteCommand(53, $"{paymentType}\t\t", ref output);
        //    //            CheckResult(protocol);

        //    //            // Close the fiscal receipt (Command 56)
        //    //            protocol.ExecuteCommand(56, "", ref output);
        //    //            CheckResult(protocol);

        //    //            Console.WriteLine("Printing successful.");
        //    //        }
        //    //        else
        //    //        {
        //    //            Console.WriteLine("Failed to open connection.");
        //    //        }
        //    //    }
        //    //    catch (FiscalPrinterException ex)
        //    //    {
        //    //        Console.WriteLine($"Fiscal Printer Error: {ex.Message} (Error Code: {ex.ErrorCode})");
        //    //        // Optionally, log the error or handle it accordingly
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Console.WriteLine($"General Exception: {ex.Message}");
        //    //    }
        //    //    finally
        //    //    {
        //    //        // Close the connection if it's open
        //    //        if (protocol.IsOpen)
        //    //        {
        //    //            protocol.CloseConnection();
        //    //        }
        //    //    }

        //    //    Console.ReadLine();
        //    //}

        //    /// <summary>
        //    /// Checks the result of the last command executed and throws an exception if there's an error.
        //    /// </summary>
        //    /// <param name="protocol">The DatecsProtocol instance.</param>
        //    static void CheckResult(DatecsProtocol protocol)
        //    {
        //        int errorCode = protocol.LastError;
        //        if (errorCode != 0)
        //        {
        //            string errorMessage = GetErrorMessage(errorCode);
        //            throw new FiscalPrinterException(errorMessage, errorCode);
        //        }
        //    }

        //    //static void Main(string[] args)
        //    //{
        //    //    FiscalPrinter fiscalPrinter = new FiscalPrinter();

        //    //    // Initialize the printer with the correct COM port and baudrate
        //    //    fiscalPrinter.InitializeFiscalPrinter("COM3", 115200);

        //    //    if (fiscalPrinter.IsConnectionOpen())
        //    //    {
        //    //        // Send a print command or any other command after initializing
        //    //        //fiscalPrinter.PrintFiscalReceipt();  // Example method from previous example
        //    //        //fiscalPrinter.OpenFiscalReceipt();
        //    //        //fiscalPrinter.TestPrintCommand();

        //    //        fiscalPrinter.ExecuteFiscalOperations();
        //    //    }

        //    //    // Don't forget to close the connection when done
        //    //    fiscalPrinter.CloseConnection();

        //    //    //USBCommunication usbComm = new USBCommunication();
        //    //    //usbComm.InitializeConnection();

        //    //    //// Send some data
        //    //    //usbComm.SendData("Hello USB!");

        //    //    //// Receive data (assuming the device sends data back)
        //    //    //usbComm.ReceiveData();

        //    //    //// Close the connection
        //    //    //usbComm.CloseConnection();

        //    //    Console.ReadLine();

        //    //    //DatecsFiscalPrinter printer = new DatecsFiscalPrinter("COM3", 115200); // Replace with your serial port name
        //    //    //printer.Open();

        //    //    //// Open fiscal receipt
        //    //    //printer.OpenFiscalReceipt(operatorId: 1, operatorPassword: "0000", tillNumber: 1);

        //    //    //// Print a sale
        //    //    //printer.PrintSale(itemName: "Item 1", price: 10.99m, quantity: 1, taxGroup: 'A');

        //    //    //// Close fiscal receipt
        //    //    //printer.CloseFiscalReceipt();

        //    //    //printer.Close();


        //    //    //// Initialize the protocol
        //    //    //DatecsProtocol protocol = new DatecsProtocol();
        //    //    //string portName = "COM3"; // Replace with your COM port
        //    //    //uint baudRate = 115200;    // Replace with your baud rate
        //    //    //string output = "";

        //    //    //try
        //    //    //{
        //    //    //    // Set up and open the connection
        //    //    //    protocol.SetRS232(portName, baudRate);
        //    //    //    protocol.OpenConnection();

        //    //    //    if (!protocol.IsOpen)
        //    //    //    {
        //    //    //        Console.WriteLine("Failed to open connection.");
        //    //    //        return;
        //    //    //    }

        //    //    //    Console.WriteLine("Connection opened successfully.");

        //    //    //    // Validate operator credentials
        //    //    //    string operatorId = "1";      // Replace with your operator ID
        //    //    //    string operatorPassword = "0000"; // Replace with your operator password

        //    //    //    protocol.ExecuteCommand(255, $"OperPasw\t{operatorId}\t\t", ref output);

        //    //    //    if (protocol.LastError == 0)
        //    //    //    {
        //    //    //        string[] response = output.Split('\t');
        //    //    //        string deviceOperatorPassword = response[1];

        //    //    //        if (operatorPassword != deviceOperatorPassword)
        //    //    //        {
        //    //    //            Console.WriteLine("Invalid operator password.");
        //    //    //            return;
        //    //    //        }
        //    //    //    }
        //    //    //    else
        //    //    //    {
        //    //    //        Console.WriteLine($"Error retrieving operator password: {protocol.LastError}");
        //    //    //        return;
        //    //    //    }

        //    //    //    // Open a fiscal receipt
        //    //    //    string receiptNumber = ""; // Empty if not specified
        //    //    //    string receiptType = "1";  // '1' for fiscal receipt
        //    //    //    string flags = "";         // Empty if not specified

        //    //    //    string openReceiptCommand = $"{operatorId}\t{operatorPassword}\t{receiptNumber}\t{receiptType}\t{flags}";
        //    //    //    protocol.ExecuteCommand(48, openReceiptCommand, ref output);

        //    //    //    if (protocol.LastError != 0)
        //    //    //    {
        //    //    //        Console.WriteLine($"Error opening receipt: {protocol.LastError}");
        //    //    //        return;
        //    //    //    }

        //    //    //    // Add an item with the name "test" to the receipt
        //    //    //    string itemName = "test";
        //    //    //    string vatGroup = "1";       // VAT group number
        //    //    //    string unitPrice = "0.00";   // Price per unit
        //    //    //    string quantity = "1";       // Quantity
        //    //    //    string additionalOptions = "[\t][\t]1[\t]"; // Additional options if any

        //    //    //    string sellItemCommand = $"{itemName}\t{vatGroup}\t{unitPrice}\t{quantity}\t{additionalOptions}";
        //    //    //    protocol.ExecuteCommand(49, sellItemCommand, ref output);

        //    //    //    if (protocol.LastError != 0)
        //    //    //    {
        //    //    //        Console.WriteLine($"Error adding item: {protocol.LastError}");
        //    //    //        return;
        //    //    //    }

        //    //    //    // Make a payment (payment type 0 typically represents cash)
        //    //    //    string paymentType = "0"; // Change if needed
        //    //    //    protocol.ExecuteCommand(53, $"{paymentType}\t[\t][\t]", ref output);

        //    //    //    if (protocol.LastError != 0)
        //    //    //    {
        //    //    //        Console.WriteLine($"Error during payment: {protocol.LastError}");
        //    //    //        return;
        //    //    //    }

        //    //    //    // Close the fiscal receipt
        //    //    //    protocol.ExecuteCommand(56, "", ref output);

        //    //    //    if (protocol.LastError != 0)
        //    //    //    {
        //    //    //        Console.WriteLine($"Error closing receipt: {protocol.LastError}");
        //    //    //        return;
        //    //    //    }

        //    //    //    Console.WriteLine("Test printed successfully.");
        //    //    //}
        //    //    //catch (Exception ex)
        //    //    //{
        //    //    //    Console.WriteLine($"Exception: {ex.Message}");
        //    //    //}
        //    //    //finally
        //    //    //{
        //    //    //    if (protocol.IsOpen)
        //    //    //    {
        //    //    //        protocol.CloseConnection();
        //    //    //        Console.WriteLine("Connection closed.");
        //    //    //    }
        //    //    //}
        //    //}

        //    /// <summary>
        //    /// Loads error messages from a file into the ErrorMessages dictionary.
        //    /// </summary>
        //    static void LoadErrorMessages()
        //    {
        //        // Assuming the error codes are stored in a file called "ErrorCodes.txt"
        //        // with the format: ErrorCode\tErrorName\tDescription
        //        try
        //        {

        //            // string[] lines = File.ReadAllLines("ErrorCodes.txt");
        //            // Split the content into lines
        //            string[] lines = Resources.ErrorCodes.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        //            foreach (string line in lines)
        //            {
        //                if (string.IsNullOrWhiteSpace(line)) continue;

        //                string[] parts = line.Split('\t');
        //                if (parts.Length >= 3)
        //                {
        //                    if (int.TryParse(parts[0], out int errorCode))
        //                    {
        //                        string description = parts[2];
        //                        if (!ErrorMessages.ContainsKey(errorCode))
        //                        {
        //                            ErrorMessages.Add(errorCode, description);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error loading error messages: {ex.Message}");
        //            // Handle the exception as needed
        //        }
        //    }

        //    /// <summary>
        //    /// Checks the result of the protocol command and throws an exception if there's an error.
        //    /// </summary>
        //    /// <param name="result">The result code from ExecuteCommand.</param>
        //    /// <param name="protocol">The DatecsProtocol instance.</param>
        //    static void CheckResult(int result, DatecsProtocol protocol)
        //    {
        //        if (result != 0)
        //        {
        //            int errorCode = protocol.LastError;
        //            string errorMessage = GetErrorMessage(errorCode);
        //            throw new FiscalPrinterException(errorMessage, errorCode);
        //        }
        //    }

        //    /// <summary>
        //    /// Returns a descriptive error message based on the error code.
        //    /// </summary>
        //    /// <param name="errorCode">The error code returned by the fiscal printer.</param>
        //    /// <returns>A string containing the error message.</returns>
        //    static string GetErrorMessage(int errorCode)
        //    {
        //        if (ErrorMessages.TryGetValue(errorCode, out string errorMessage))
        //        {
        //            return errorMessage;
        //        }
        //        else
        //        {
        //            return $"Unknown error (Code: {errorCode}).";
        //        }
        //    }
        //}

        ///// <summary>
        ///// Custom exception class for fiscal printer errors.
        ///// </summary>
        //public class FiscalPrinterException : Exception
        //{
        //    public int ErrorCode { get; private set; }

        //    public FiscalPrinterException(string message, int errorCode) : base(message)
        //    {
        //        this.ErrorCode = errorCode;
        //    }
        //}
    }
}
