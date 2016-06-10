/*
Cube-Robot Arduino 
This takes commands over the serial port and outputs the correct commands to the servos
Created June 9, 2016
by Richard Arkusinski
*/

#include <Servo.h> 

int cTime = 12; // The time to complete each command in ms
int pause = 100; // The time delay between commands in ms

//Assuming openPosition is > closedPosition
int openPosition = 50; // The open position of the claw
int closedPosition = 0; // The closed position of the claw

// Create servo objects
Servo rightClaw, rightRotate, leftClaw, leftRotate;

void setup(){
  // Open Serial Connection
  Serial.begin(9600);
  
  // Attaches the servos to pins
  rightClaw.attach(A0);
  rightRotate.attach(A1);
  leftClaw.attach(A2);
  leftRotate.attach(A3); 
}

void loop(){
  
  delay(1);
}

boolean rotate(boolean side, int angle){
  
  return true; 
}

// Opens or closes one of the claws
// When cOpen is true it opens the claw, and when it is false it closes the claw
// for side, true is left, fase is right
boolean claw(boolean side, boolean cOpen){
  float delayTime = (openPosition-closedPosition)/cTime;
  if(cOpen){ //Opening claw
    for(int pos = closedPosition; pos < openPosition; pos+= 1){
      if(side){ // left claw
        leftClaw.write(pos);  
      }else{ // right claw
        rightClaw.write(pos);
      }
      delay(delayTime);
    }
  }else{ //Closing claw
    for(int pos = openPosition; pos > closedPosition; pos-= 1){
      if(side){ // left claw
        leftClaw.write(pos);  
      }else{ //right claw
        rightClaw.write(pos);
      }
      delay(delayTime);
    }
  }
  return true;
}  
