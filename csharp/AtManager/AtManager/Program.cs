using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.PlatformServices;
using System.Threading;
using System.Reactive.Threading.Tasks;

namespace AtManager
{
    class Program
    {
        private static SerialPort _serialPort;
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        //
        // AT commands you would like to send as Initialization package
        //
        private static List<string> serverInitializationStack = new List<string>
        {
            "AT+RST\r\n",
        };

        static void Main(string[] args)
        {
            _serialPort = new SerialPort("COM5");
            _serialPort.BaudRate = 115200;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.Handshake = Handshake.None;

            try
            {
                _serialPort.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Application had an exception should be closed. Press any key to continue...");
                Console.ReadLine();
                return;
            }

            InitializeWifiNetworkAndServer();


            ManualCommandOperation();

            _serialPort.Close();
        }

        private static void InitializeWifiNetworkAndServer()
        {
            var atCommandService = new AtCommandService(_serialPort, cancellationTokenSource.Token);

            // Send all commands to initialize wifi network and server
            foreach (var command in serverInitializationStack)
            {
                var message = atCommandService.SendCommand(command, 5000, true);
            }
        }

        private static void ManualCommandOperation()
        {
            Console.WriteLine("Write 'END' when you will wish to close this application.");
            Console.WriteLine("To send command press enter (remember that \\r\\n characters will be added automatically to end of the command).\n");
            Console.WriteLine("Please type '<CR>' after each command to be send.\n");

            var atCommandService = new AtCommandService(_serialPort, cancellationTokenSource.Token);

            // After that wait for any manuall commands typed by administrator
            while (true)
            {
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                var outdata = Console.ReadLine();

                if (!outdata.StartsWith("END"))
                {
                    outdata = outdata.Replace("<CR>", "\r\n");
                    if (_serialPort.IsOpen)
                    {
                        var message = atCommandService.SendCommand(outdata + "\r\n", 5000, false);
                    }
                    else
                    {
                        Console.WriteLine("Serial port was closed will try to reopen it within 5 seconds. Press any key to continue...");
                        Console.ReadLine();

                        //serialPort.Close();
                        _serialPort.Open();

                        Thread.Sleep(5000);
                    }
                }
                else
                    break;

                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
            }
        }

        private static string indata = string.Empty;

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            indata += sp.ReadExisting();
            if (indata.Contains("OK\\r\\n") || indata.Contains("ERROR\\r\\n"))
            {

                indata = indata.Replace("\r", @"\r");
                indata = indata.Replace("\n", @"\n");

                Console.WriteLine($"<< {indata}\n");

                // DO SOMETHING WITH INCOMING DATA

                indata = string.Empty;
            }
        }
    }
}
