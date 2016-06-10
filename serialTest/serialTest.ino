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
  digitalWrite(13, HIGH);
  
  // Receives serial data 
  // Turns off LED to indicate data was received
  if (Serial.available() > 0) {
    dataIn = Serial.readString();   
    digitalWrite(13, LOW);  
  }

  // Sends serial data
  Serial.print(dataIn);
 
  delay(1000);              
}
