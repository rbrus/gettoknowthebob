using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Reactive.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace PEndDevice
{
    public class AtCommandService
    {
        private SerialPort _serialPort;
        private CancellationToken _cancel;

        public AtCommandService(SerialPort serialPort, CancellationToken cancel)
        {
            _cancel = cancel;
            _serialPort = serialPort;
        }

        private IObservable<object> _dataReceivedSubscription
        {
            get
            {
                return Observable
                    .FromEventPattern<SerialDataReceivedEventHandler, SerialDataReceivedEventArgs>(x => _serialPort.DataReceived += x, x => _serialPort.DataReceived -= x)
                    .Select(x => x.Sender);
            }
        }

        public Message SendCommand(string command, int timeoutPeriod, bool printDebug)
        {
            if (command == string.Empty) return Message.EmptyMessage;

            Console.WriteLine($">> {command}");

            var msg = new Message(command);

            var responsetask = _dataReceivedSubscription.Subscribe(x => msg.ProcessIncomingData(((SerialPort)x).ReadExisting()));

            if (_serialPort.IsOpen)
                _serialPort.WriteLine(command);
            else
                Console.WriteLine($"## {command} serialPort close()");

            while (timeoutPeriod > 0)
            {
                timeoutPeriod -= 500;
                Thread.Sleep(500);

                if (msg.ResponseValue != ResponseValue.NotReady)
                    break;
            }

            responsetask.Dispose();

            if (printDebug)
            {
                Console.WriteLine($"<< {msg.ReceivedString}");

                if (msg.ResponseValue == ResponseValue.OK)
                    Console.WriteLine($"## [{command}] successfully delivered");
                else
                    Console.WriteLine($"## [{command}] was unable to send command : {msg.ResponseValue}");

                Console.WriteLine();
            }

            return msg;
        }

        private void HandleException(Exception exception)
        {
            Console.WriteLine($"## HandleException : {exception.Message}");
        }
    }
}
