/*
Cube-Robot Arduino 
This takes commands over the serial port and outputs the correct commands to the servos
Created June 9, 2016
by Richard Arkusinski
*/

#include <Servo.h> 

int cTime = 12; // The time to complete each claw command in ms
int rTime = 12; // The time to complete each rotation command in ms
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
  //Check for serial inputs
  while (Serial.available() > 0){
    //Read the serial input until we get to a space
    String input = Serial.readStringUntil(' ');
    int inputInt = mapInput(input);
    switch (inputInt){
      case 0:
        Serial.println("Unrecognized command"); 
        break;
      case 1: //Open right claw
        Serial.println("Open right claw"); 
        claw(false, true);
        break;
      case 2: //Open left claw
        Serial.println("Open left claw"); 
        claw(true, true);
        break;
      case 3: //Close right claw
        Serial.println("Close right claw"); 
        claw(false, false);
        break;
      case 4: //Close left claw
        Serial.println("Close left claw"); 
        claw(true, false);
        break;
      case 5: //Rotate right claw clockwise
        Serial.println("Rotate right claw clockwise"); 
        rotate(false, 90);
        break;
      case 6: //Rotate right claw counterclockwise
        Serial.println("Rotate right claw counterclockwise"); 
        rotate(false, -90);
        break;
      case 7: //Rotate left claw clockwise
        Serial.println("Rotate left claw clockwise"); 
        rotate(true, 90);
        break;
      case 8: //Rotate left claw counterclockwise
        Serial.println("Rotate left claw counterclockwise"); 
        rotate(true, -90);
        break;
      default:
        Serial.println("COSMIC RAYS AHOY"); break;
    }
  }
}

// Maps the input string to an int so it can be used in a switch statement 
int mapInput(String input){
  if (input == "open2") return 1; //Open right claw
  else if (input == "open1") return 2; //Open left claw
  else if (input == "close2") return 3; //Close right claw
  else if (input == "close1") return 4; //Close left claw
  else if (input == "2") return 5; //Rotate right claw clockwise
  else if (input == "2p") return 6; //Rotate right claw counterclockwise
  else if (input == "1") return 7; //Rotate left claw clockwise
  else if (input == "1p") return 8; //Rotate left claw counterclockwise
  else return 0; // Unrecognized command
}

//Store the current rotations 
int lPos = 0;
int rPos = 0;

// Rotates the one of the claws
// for side, true is left, false is right
// direct is how many degrees you want to move
boolean rotate(boolean side, int direct){
  float delayTime = abs(direct/rTime);
  if(side){ // Left claw
    for(int pos = lPos; Sign(direct)*pos < Sign(direct)*(lPos + direct); pos +=Sign(direct)){
      leftRotate.write(pos);
      lPos = pos;
    }
  }else{ // Right claw
    for(int pos = rPos; Sign(direct)*pos < Sign(direct)*(lPos + direct); pos +=Sign(direct)){
      rightRotate.write(pos);
      rPos = pos;
    }
  }
  return true; 
}

// Opens or closes one of the claws
// for side, true is left, fase is right
// When cOpen is true it opens the claw, and when it is false it closes the claw
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

// Custom signum function
int Sign(int num){
  if (num > 0) return 1;
  else if (num <0) return -1;
  else return 0;
}
