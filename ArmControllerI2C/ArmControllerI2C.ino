/*
 Name:		ArmControllerI2C.ino
 Created:	08-Jan-17 16:46:13
 Author:	DWEIS
*/

#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

int pwm_a = 8;  //PWM control for motor outputs 1 and 2
int dir_a = 9;  //direction control for motor outputs 1 and 2

const byte baseIndex = 0;
const uint16_t baseMin = 100;
const uint16_t baseMax = 600;

const byte shoulderIndex = 1;
const uint16_t shoulderMin = 100;
const uint16_t shoulderMax = 600;

const byte elbowIndex = 2;
const uint16_t elbowMin = 100;
const uint16_t elbowMax = 600;

uint16_t CurrentBaseAngle = 300;
uint16_t DesiredBaseAngle = 300;

uint16_t CurrentShoulderAngle = 300;
uint16_t DesiredShoulderAngle = 300;

uint16_t CurrentElbowAngle = 300;
uint16_t DesiredElbowAngle = 300;

const uint16_t maxStep = 1;
const unsigned long updateTimeout = 8;
unsigned long nextUpdate = 0;

Adafruit_PWMServoDriver pwmController = Adafruit_PWMServoDriver();

// the setup function runs once when you press reset or power the board
void setup() {
	Serial.begin(9600);
	pwmController.begin();
	pwmController.setPWMFreq(60);
	// magnet controller
	pinMode(pwm_a, OUTPUT);  //Set control pins to be outputs
	pinMode(dir_a, OUTPUT);
	digitalWrite(dir_a, LOW);
	analogWrite(pwm_a, 0);
}

// the loop function runs over and over again until power down or reset
void loop() {
	ReadServoCommands();
	UpdateServos();
}

void ReadServoCommands() {
	if (Serial.available() > 2) {
		byte index = Serial.read();
		byte high = Serial.read();
		byte low = Serial.read();
		if (index == 20) {
			analogWrite(pwm_a, 255);
			return;
		}
		if (index == 21) {
			analogWrite(pwm_a, 0);
			return;
		}
		uint16_t desiredAngle = (high << 8) | low;
		switch (index) {
		case baseIndex:
			DesiredBaseAngle = constrain(desiredAngle, baseMin, baseMax);
			break;
		case shoulderIndex:
			DesiredShoulderAngle = constrain(desiredAngle, shoulderMin, shoulderMax);
			break;
		case elbowIndex:
			DesiredElbowAngle = constrain(desiredAngle, elbowMin, elbowMax);
			break;
		}
	}
}

void UpdateServos() {
	if (nextUpdate < millis()) {
		nextUpdate = millis() + updateTimeout;
		UpdateServoValues();
		SetServoValues();
	}
}

void SetServoValues() {
	pwmController.setPWM(baseIndex, 0, CurrentBaseAngle);
	pwmController.setPWM(shoulderIndex, 0, CurrentShoulderAngle);
	pwmController.setPWM(elbowIndex, 0, CurrentElbowAngle);
}

void UpdateServoValues() {
	// Update Base
	if (DesiredBaseAngle > CurrentBaseAngle) {
		uint16_t difference = DesiredBaseAngle - CurrentBaseAngle;
		if (difference <= maxStep) {
			DesiredBaseAngle = CurrentBaseAngle;
		}
		else {
			CurrentBaseAngle += maxStep;
		}
	}
	else if (CurrentBaseAngle > DesiredBaseAngle) {
		uint16_t difference = CurrentBaseAngle - DesiredBaseAngle;
		if (difference <= maxStep) {
			DesiredBaseAngle = CurrentBaseAngle;
		}
		else {
			CurrentBaseAngle -= maxStep;
		}
	}
	// Update shoulder
	if (DesiredShoulderAngle > CurrentShoulderAngle) {
		uint16_t difference = DesiredShoulderAngle - CurrentShoulderAngle;
		if (difference <= maxStep) {
			DesiredShoulderAngle = CurrentShoulderAngle;
		}
		else {
			CurrentShoulderAngle += maxStep;
		}
	}
	else if (CurrentShoulderAngle > DesiredShoulderAngle) {
		uint16_t difference = CurrentShoulderAngle - DesiredShoulderAngle;
		if (difference <= maxStep) {
			DesiredShoulderAngle = CurrentShoulderAngle;
		}
		else {
			CurrentShoulderAngle -= maxStep;
		}
	}
	// Update elbows
	if (DesiredElbowAngle > CurrentElbowAngle) {
		uint16_t difference = DesiredElbowAngle - CurrentElbowAngle;
		if (difference <= maxStep) {
			DesiredElbowAngle = CurrentElbowAngle;
		}
		else {
			CurrentElbowAngle += maxStep;
		}
	}
	else if (CurrentElbowAngle > DesiredElbowAngle) {
		uint16_t difference = CurrentElbowAngle - DesiredElbowAngle;
		if (difference <= maxStep) {
			DesiredElbowAngle = CurrentElbowAngle;
		}
		else {
			CurrentElbowAngle -= maxStep;
		}
	}
}
