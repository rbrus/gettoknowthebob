#include <iostream>
#include <list>
#include <wiringPi.h>
#include <wiringSerial.h>

using namespace std;

void sendCommand(int connectionId, char *command, int waitTime, bool debug);
//http://wiringpi.com/reference/serial-library/

int main()
{
    char * commandStack[] =
    {
        "AT\r\n",
        "AT+RST\r\n",
        "AT+GMR\r\n",
        "AT+CWMODE=1\r\n"
        "AT+CWLAP\r\n",
        //"AT+CWJAP=\"Fred\'s Network\",\"TestPasswordNumber2#\"\r\n",
        "AT+CIFSR\r\n"
    };

    wiringPiSetup();
    int connectionId = serialOpen("/dev/ttyAMA0", 115200);
    int dataAvailable;

    for(int i=0 ; i < 7 ;i++)
    {
        //const char * c = commandStack[i].c_str();
        sendCommand(connectionId, commandStack[i], 3000, true);
    }

    //dataAvailable = serialDataAvail(connectionId);
    //while(dataAvailable == 0)
    //{
    //    dataAvailable = serialDataAvail(connectionId);
    //    //if nothing to read something went wrong
    //}

    serialClose(connectionId);

    return 0;
}

// Time to wait for response (in ms). If debug is set to true command string will be printed
void sendCommand(int connectionId, char *command, int waitTime, bool debug)
{
    // let's flush remaining data
    serialFlush(connectionId);

    if(debug)
        cout << "Command:" << command << endl << "Data: ";

    // send data
    serialPuts(connectionId, command);

    int timer = millis()+waitTime+1;

    while(timer > millis())
    {
        if(serialDataAvail(connectionId)> 0)
        {
            while(serialDataAvail(connectionId)> 0)
            {
                cout << (char)serialGetchar(connectionId);
            }
        }
    }

    cout << endl;
}

