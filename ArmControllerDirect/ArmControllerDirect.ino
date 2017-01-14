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

const uint8_t SERVO_SPEED = 10;

const byte setServoCommand = 0;
const byte setAllCommand = 1;
const byte setMagnetCommand = 2;

const byte baseIndex = 0;
const byte ShoulderIndex = 1;
const byte ElbowIndex = 2;

VarSpeedServo Base;
VarSpeedServo Shoulder;
VarSpeedServo Elbow;

unsigned long previousMillis = 0;
const long updateInterval = 2000;

void setup() {
	pinMode(magnetPin, OUTPUT);
	digitalWrite(magnetPin, LOW);
	Shoulder.attach(shoulderServoPin);
	delay(1000);
	Elbow.attach(elbowServoPin);
	delay(1000);
	Base.attach(baseServoPin);
	Serial.begin(9600);
	while (!Serial);

	Base.write(90, 10, true);
	Shoulder.write(90, 10, true);
	Elbow.write(90, 10, true);
	byte testArray[] = { 42, 96, 42, 96, 42, 96, 42, 96, 42 };
	Serial.write(testArray, sizeof(testArray));
	Serial.flush();
}

void loop() {
	if (Serial.available())
	{
		byte command = Serial.read();
		switch (command)
		{
		case setServoCommand:
			SetServo();
			break;
		case setAllCommand:
			SetAll();
			break;
		case setMagnetCommand:
			SetMagnet();
			break;
		}
	}
	unsigned long currentMillis = millis();
	if (currentMillis - previousMillis >= updateInterval)
	{
		previousMillis = currentMillis;
		byte byteArray[9];
		uint16_t baseMicro = Base.readMicroseconds();
		byteArray[0] = baseIndex;
		byteArray[1] = ShoulderIndex;
		byteArray[2] = ElbowIndex;
		byteArray[3] = (baseMicro >> 8) & 0xFF;
		byteArray[4] = baseMicro & 0xFF;
		uint16_t shoulderMicro = Shoulder.readMicroseconds();
		byteArray[5] = (shoulderMicro >> 8) & 0xFF;
		byteArray[6] = shoulderMicro & 0xFF;
		uint16_t elbowMicro = Elbow.readMicroseconds();
		byteArray[7] = (elbowMicro >> 8) & 0xFF;
		byteArray[8] = elbowMicro & 0xFF;
		Serial.write(byteArray, 9);
		Serial.flush();
	}
}

void SetServo() {
	while (Serial.available() < 3);
	byte index = Serial.read();
	byte high = Serial.read();
	byte low = Serial.read();
	uint16_t pulsWidth = (high << 8) | low;
	switch (index)
	{
	case baseIndex:
		Base.write(pulsWidth, SERVO_SPEED);
		break;
	case ShoulderIndex:
		Shoulder.write(pulsWidth, SERVO_SPEED);
		break;
	case ElbowIndex:
		Elbow.write(pulsWidth, SERVO_SPEED);
		break;
	}
}

void SetAll() {
	while (Serial.available() < 6);
	byte high = Serial.read();
	byte low = Serial.read();
	uint16_t pulsBase = (high << 8) | low;
	high = Serial.read();
	low = Serial.read();
	uint16_t pulsShoulder = (high << 8) | low;
	high = Serial.read();
	low = Serial.read();
	uint16_t pulsElbow = (high << 8) | low;
	Base.write(pulsBase, SERVO_SPEED);
	Shoulder.write(pulsShoulder, SERVO_SPEED);
	Elbow.write(pulsElbow, SERVO_SPEED);
}

void SetMagnet() {
	while (Serial.available() < 1);
	byte on = Serial.read();
	if (on)
	{
		digitalWrite(magnetPin, HIGH);
	}
	else {
		digitalWrite(magnetPin, LOW);
	}
}
