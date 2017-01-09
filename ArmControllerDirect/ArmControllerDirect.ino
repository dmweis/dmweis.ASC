/*
 Name:		ArmControllerDirect.ino
 Created:	08-Jan-17 16:45:54
 Author:	DWEIS
*/

#include <VarSpeedServo.h>

const int magnetPin = 8;
const int baseServoPin = 6;
const int shoulderServoPin = 3;
const int elbowServoPin = 5;

const byte baseIndex = 0;
const byte shoulderIndex = 1;
const byte elbowIndex = 2;

VarSpeedServo Base;
VarSpeedServo Shoulder;
VarSpeedServo Elbow;


void setup() {
	pinMode(magnetPin, OUTPUT);
	digitalWrite(magnetPin, LOW);
	Base.attach(baseServoPin);
	Shoulder.attach(shoulderServoPin);
	Elbow.attach(elbowServoPin);
	Serial.begin(9600);
	while (!Serial);

	Base.write(90, 10, true);
	Shoulder.write(90, 10, true);
	Elbow.write(90, 10, true);
}

void loop() {
	if (Serial.available() > 2)
	{
		byte index = Serial.read();
		byte high = Serial.read();
		byte low = Serial.read();
		uint16_t pulsWidth = (high << 8) | low;
		switch (index)
		{
		case baseIndex:
			Base.write(pulsWidth, 10);
			break;
		case shoulderIndex:
			Shoulder.write(pulsWidth, 10);
			break;
		case elbowIndex:
			Elbow.write(pulsWidth, 10);
			break;
		case 20:
			digitalWrite(magnetPin, HIGH);
			break;
		case 21:
			digitalWrite(magnetPin, LOW);
			break;
		}
	}
}
