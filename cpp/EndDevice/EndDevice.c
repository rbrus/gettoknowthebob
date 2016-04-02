#include <iostream>
#include <list>
#include <wiringPi.h>
#include <wiringSerial.h>
using namespace std;
void sendCommand(int connectionId, char *command, int waitTime, bool debug);

int main()
{
    std::list<char *> commandList = {
        "AT+RST\r\n",
        "AT+CWMODE=1\r\n",
        "AT+CWJAP=\"TestSSID\",\"Test.Password#\"\r\n",
        "AT+CIFSR\r\n",
        "AT+CIPSTART=\"TCP\",\"192.168.4.1\",80\r\n",
        "AT+CIPSEND=4\r\n",
        "ABCD"
    };

    wiringPiSetup();
    int connectionId = serialOpen("/dev/ttyAMA0", 115200);

    cout << "Number of commands: " << commandList.size() << endl;

    // Send all commands from stack
    while(commandList.size() != 0)
    {
        //const char * c = commandStack[i].c_str();
        sendCommand(connectionId, commandList.front(), 3000, true);
        commandList.pop_front();
    }

    bool notClose = true;
    int x = 0; //
    char indata[8192];
    while(notClose)
    {
        if(serialDataAvail(connectionId) > 0)
        {
            while(serialDataAvail(connectionId) > 0)
            {
                char c = (char)serialGetchar(connectionId);
                cout << c;
                indata[x] = c;
                indata++;
            }
        }

        if(x > 3)
        {
            if(indata[x - 1] == '\r' && indata[x] == '\n')
            {
                if(indata[x - 3] == 'O' && indata[x - 2] == 'K')
                {

                }

                if(indata[x - 3] == 'O' && indata[x - 2] == 'R')
                {

                }
            }
        }
    }

    serialClose(connectionId);

    return 0;
}

// Time to wait for response (in ms). If debug is set to true command string will be printed
void sendCommand(int connectionId, char *command, int waitTime, bool debug)
{
    // let's flush remaining data
    serialFlush(connectionId);

    if(debug)
    {
        cout << "----------------------------------" << endl;
        cout << "Command:" << command << endl << "Data: ";
    }
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

