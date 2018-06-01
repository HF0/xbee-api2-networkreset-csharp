using System;
using System.IO.Ports;

namespace resetXbee
{
    // issues a NR0 AT command to an Xbee (API 2)
    class Program
    {
        private const int BAUD_RATE = 9600;
        // NR0 AT command
        private static readonly byte[] NR_API2 = new byte[]{ 0x7E, 0x00, 0x05, 0x08, 0x01, 0x4E, 0x52, 0x00, 0x56 };
        // NR0 expected response (OK response)
        private static readonly byte[] NR_EXPECTED_RESPONSE_API2 = new byte[] { 0x7E, 0x00, 0x05, 0x88, 0x01, 0x4E, 0x52, 0x00, 0xD6 };

        private static string getChosenSerialPortName()
        {
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("Xbee Network reset in API mode (9600 baudrate is used):");
            Console.WriteLine("Available COM ports:");

            int menuNumber = 1;
            foreach (string port in ports)
            {
                Console.WriteLine(menuNumber + ". " + port);
                menuNumber++;
            }
            Console.WriteLine("Please select the COM port where the xbee is connected");

            string option = Console.ReadLine().Trim();
            int optionNumber = int.Parse(option);
            optionNumber--;

            if (optionNumber < 0 || optionNumber >= ports.Length)
            {
                throw new FormatException();
            }

            string portName = ports[optionNumber];
            return portName;
        }

        public static void Main()
        {
            try
            {
                string portName = getChosenSerialPortName();
                Console.WriteLine("Opening port " + portName);
                SerialPort serial = new SerialPort(portName, BAUD_RATE);
                serial.Open();
                if(!serial.IsOpen)
                {
                    Console.WriteLine("Error opening port");
                    return;
                }
                Console.WriteLine("Sending NR0"); 
                serial.Write(NR_API2, 0, NR_API2.Length);
                bool command_ok = true;
                Console.WriteLine("Waiting for response");
                for ( int i=0; command_ok && i < NR_EXPECTED_RESPONSE_API2.Length; i++)
                {
                    if( serial.ReadByte() != NR_EXPECTED_RESPONSE_API2[i])
                    {
                        command_ok = false;
                    }
                }

                if( command_ok)
                {
                    Console.WriteLine("Reset ok");
                }else
                {
                    Console.WriteLine("Error resetting network");
                }
                serial.Close();
                Console.WriteLine("Finished");
                Console.ReadLine();
            }
            catch(FormatException)
            {
                Console.WriteLine("Error: option not valid");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
        }
    }
}
