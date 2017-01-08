/*
 Name:		ArmControllerDirect.ino
 Created:	08-Jan-17 16:45:54
 Author:	DWEIS
*/

#include <VarSpeedServo.h>

VarSpeedServo myservo;    // create servo object to control a servo


// the setup function runs once when you press reset or power the board
void setup() {
	myservo.attach(9);  // attaches the servo on pin 9 to the servo object
	Serial.begin(9600);
}

// the loop function runs over and over again until power down or reset
void loop() {
	//myservo.write(180, 10, true);        // move to 180 degrees, use a speed of 30, wait until move is complete
	//myservo.write(0, 255, true);        // move to 0 degrees, use a speed of 30, wait until move is complete
	if (Serial.available() > 1)
	{
		int pos = Serial.parseInt();
		int speed = Serial.parseInt();
		Serial.print("position: ");
		Serial.print(pos);
		Serial.print(" speed: ");
		Serial.println(speed);
		myservo.write(pos, speed);
		int currentPos = -1;
		while (currentPos != pos)
		{
			currentPos = myservo.read();
			Serial.print("Current position: ");
			Serial.println(currentPos);
			delay(50);
		}
		Serial.println("Done");
	}
}
