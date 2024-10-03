using System;
using UDatecsProtocol;

namespace FP700
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatecsFiscalPrinter printer = new DatecsFiscalPrinter("COM10", 115200); // Replace with your serial port name
            printer.Open();

            // Open fiscal receipt
            printer.OpenFiscalReceipt(operatorId: 1, operatorPassword: "1234", tillNumber: 1);

            // Print a sale
            printer.PrintSale(itemName: "Item 1", price: 10.99m, quantity: 1, taxGroup: 'A');

            // Close fiscal receipt
            printer.CloseFiscalReceipt();

            printer.Close();


            //// Initialize the protocol
            //DatecsProtocol protocol = new DatecsProtocol();
            //string portName = "COM10"; // Replace with your COM port
            //uint baudRate = 115200;    // Replace with your baud rate
            //string output = "";

            //try
            //{
            //    // Set up and open the connection
            //    protocol.SetRS232(portName, baudRate);
            //    protocol.OpenConnection();

            //    if (!protocol.IsOpen)
            //    {
            //        Console.WriteLine("Failed to open connection.");
            //        return;
            //    }

            //    Console.WriteLine("Connection opened successfully.");

            //    // Validate operator credentials
            //    string operatorId = "1";      // Replace with your operator ID
            //    string operatorPassword = "0000"; // Replace with your operator password

            //    protocol.ExecuteCommand(255, $"OperPasw\t{operatorId}\t\t", ref output);

            //    if (protocol.LastError == 0)
            //    {
            //        string[] response = output.Split('\t');
            //        string deviceOperatorPassword = response[1];

            //        if (operatorPassword != deviceOperatorPassword)
            //        {
            //            Console.WriteLine("Invalid operator password.");
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Error retrieving operator password: {protocol.LastError}");
            //        return;
            //    }

            //    // Open a fiscal receipt
            //    string receiptNumber = ""; // Empty if not specified
            //    string receiptType = "1";  // '1' for fiscal receipt
            //    string flags = "";         // Empty if not specified

            //    string openReceiptCommand = $"{operatorId}\t{operatorPassword}\t{receiptNumber}\t{receiptType}\t{flags}";
            //    protocol.ExecuteCommand(48, openReceiptCommand, ref output);

            //    if (protocol.LastError != 0)
            //    {
            //        Console.WriteLine($"Error opening receipt: {protocol.LastError}");
            //        return;
            //    }

            //    // Add an item with the name "test" to the receipt
            //    string itemName = "test";
            //    string vatGroup = "1";       // VAT group number
            //    string unitPrice = "0.00";   // Price per unit
            //    string quantity = "1";       // Quantity
            //    string additionalOptions = "[\t][\t]1[\t]"; // Additional options if any

            //    string sellItemCommand = $"{itemName}\t{vatGroup}\t{unitPrice}\t{quantity}\t{additionalOptions}";
            //    protocol.ExecuteCommand(49, sellItemCommand, ref output);

            //    if (protocol.LastError != 0)
            //    {
            //        Console.WriteLine($"Error adding item: {protocol.LastError}");
            //        return;
            //    }

            //    // Make a payment (payment type 0 typically represents cash)
            //    string paymentType = "0"; // Change if needed
            //    protocol.ExecuteCommand(53, $"{paymentType}\t[\t][\t]", ref output);

            //    if (protocol.LastError != 0)
            //    {
            //        Console.WriteLine($"Error during payment: {protocol.LastError}");
            //        return;
            //    }

            //    // Close the fiscal receipt
            //    protocol.ExecuteCommand(56, "", ref output);

            //    if (protocol.LastError != 0)
            //    {
            //        Console.WriteLine($"Error closing receipt: {protocol.LastError}");
            //        return;
            //    }

            //    Console.WriteLine("Test printed successfully.");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Exception: {ex.Message}");
            //}
            //finally
            //{
            //    if (protocol.IsOpen)
            //    {
            //        protocol.CloseConnection();
            //        Console.WriteLine("Connection closed.");
            //    }
            //}
        }
    }
}
