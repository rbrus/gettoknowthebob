#include <iostream>
#include <list>
#include <wiringPi.h>
#include <wiringSerial.h>

using namespace std;


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
        //"AT+CWJAP=\"Fred\'s Network\",\"Leos12072014#\"\r\n",
        "AT+CIFSR\r\n"
    };

    wiringPiSetup();
    int connection = serialOpen("/dev/ttyAMA0", 115200);

    for(int i=0 ; i < 7 ;i++)
    {

        //const char * c = commandStack[i].c_str();
        serialPuts(connection, commandStack[i]);
        cout << commandStack[i] << endl;

    }

    return 0;
}
