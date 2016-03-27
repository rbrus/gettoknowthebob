console.log("-- Begin");

var list = [
    "AT\r\n",
    "AT+CWMODE=1\r\n",
    "AT+CWLAP\r\n",
    "AT+CWJAP=\"Fred\'s Network\",\"Leos12072014#\""
];


var start = 0;
var allData;

var SerialPort = require('serialport').SerialPort;

    var serialPort = new SerialPort('/dev/ttyAMA0', {
      baudrate: 115200,
      dataBits: 8,
      parity: 'none',
      stopBits: 1,
      flowControl: false
    });

    serialPort.open( function(error)
    {
      if(error)
      {
        console.log('failed to open: ' + error);
      }
      else
      {
        console.log('open serialport');

        serialPort.on('data', function(data)
        {
          console.log("data received: " + data);
          if(data == 'AT+CWLAP\r\n')
          {
            start = 1;
          }
          if(start == 1)
          {
            if(data.endsWith('OK\r\n'))
            {
                console.log(allData);
            }
            allData += data;
          }
        });

        for (var i = 0; i < 3; i++ )
        {   //this is the outer loop
            serialPort.write(list[i], function(err, results)
            {
              console.log('err ' + err);
              console.log('results ' + results);
            });
        }
}
    });

console.log("-- End");

