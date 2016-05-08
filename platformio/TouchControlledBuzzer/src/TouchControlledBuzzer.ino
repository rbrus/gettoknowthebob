#include <Arduino.h>

int returnValue = 0;

void setup()  { 
  Serial.begin(115200);
  pinMode(2, INPUT);
  pinMode(4, OUTPUT);
  delay(1000);
} 

void loop()  { 
  returnValue = analogRead(2);
  Serial.println("Sensor value: " + String(returnValue));
  delay(100);
  if(returnValue > 500)
    analogWrite(4,1);
  else
    analogWrite(4,0);
}  
 
