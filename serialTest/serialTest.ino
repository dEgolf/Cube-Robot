// Intercept serial command and blink LED

// Store data sent to Arduino over serial line
String dataIn = "";

void setup() {
  // Open serial port on arduino end
  Serial.begin(9600); 
  
  // Initialize digital pin 13 as an output.
  pinMode(13, OUTPUT);
  
  // Turn the LED on (HIGH is the voltage level)
  digitalWrite(13, HIGH);   
}

void loop() {

  // Receives data sent
  // Turns off LED to indicate data was received
  if (Serial.available() > 0) {
    dataIn = Serial.readString();   
    digitalWrite(13, LOW);    /
    delay(1000);               
    digitalWrite(13, HIGH);
  }
 
  delay(1000);              
}
