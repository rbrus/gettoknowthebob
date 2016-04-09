using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace PEndDevice
{
    public class Message
    {
        private string _tempDataContainer = string.Empty;
        public static Message EmptyMessage = new Message(string.Empty);

        public string ReceivedString { get; private set; }
        public string CommandString { get; private set; }
        public ResponseValue ResponseValue { get; private set; }

        public Message(string command)
        {
            CommandString = command;
            ResponseValue = ResponseValue.NotReady;
        }

        public void ProcessIncomingData(string incomingData)
        {
            _tempDataContainer += incomingData;
            ReceivedString = ProcessIncommingData(_tempDataContainer);
            ResponseValue = GetResponseValue(_tempDataContainer);
        }

        private ResponseValue GetResponseValue(string data)
        {
            if (data.Contains("OK\r\n"))
                return ResponseValue.OK;
            else if (data.Contains("ERROR\r\n"))
                return ResponseValue.ERROR;

            return ResponseValue.NotReady;
        }

        private string ProcessIncommingData(string data)
        {
            data = data.Replace("\r", @"\r");
            return data.Replace("\n", @"\n");
        }
    }
}
