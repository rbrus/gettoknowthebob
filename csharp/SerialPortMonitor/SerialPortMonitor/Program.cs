using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SerialPortMonitor
{
    class Program
    {
        private static string indata = string.Empty;

        static void Main(string[] args)
        {
            var indata = string.Empty;
            SerialPort serialPort = new SerialPort("COM5");
            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try {
                serialPort.Open();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine("Application had an exception and must be closed. Press any key to continue...");
                Console.ReadLine();
                return;
            }
            
            Console.WriteLine("Write 'end' when you will wish to close this application.");
            Console.WriteLine("To send command press enter (remember that \\r\\n characters will be added automatically to end of the command).\n\n");
            while (true) {
                indata = Console.ReadLine();
                if (!indata.StartsWith("end")) serialPort.WriteLine(indata + "\r\n");
                else break;
            }

            serialPort.Close();
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            indata += sp.ReadExisting();
            
            if (indata.Contains("OK\r\n") || indata.Contains("ERROR\r\n"))
            {
                Console.WriteLine($"<< {indata}");
                // DO SOMETHING WITH DATA
                indata = string.Empty;
            }
        }
    }
}
