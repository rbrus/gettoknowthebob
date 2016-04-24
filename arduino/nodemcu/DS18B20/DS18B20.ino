#include <ESP8266WiFi.h>
#include <OneWire.h>
#include <DallasTemperature.h>

#define myPeriodic 15   // Seconds
#define ONE_WIRE_BUS 2  // DS18B20 on arduino pin2 corresponds to D4 on physical board

OneWire oneWire(ONE_WIRE_BUS);
DallasTemperature DS18B20(&oneWire);
float prevTemp = 0;
const char* MY_SSID = "Fred's Network"; 
const char* MY_PWD = "Leos12072014#";
int sent = 0;
void setup() {
  Serial.begin(115200);
}

void loop() {
  float temp;
  DS18B20.requestTemperatures(); 
  temp = DS18B20.getTempCByIndex(0);
  Serial.print(String(sent)+" Temperature: ");
  Serial.println(temp);
  sent++;
  int count = myPeriodic;
  while(count--)
  delay(1000);
}

