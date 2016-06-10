using System;
using System.Collections.Generic;
using System.Diagnostics;  // To use Process.Start
using System.IO.Ports;

namespace ConsoleApplication1
{
    internal class Program
    {
        static SerialPort myPort;
        static bool doSerial = false; // Controls whether we send to Arduino or just print to screen

        // Tries to open port "portName", if it isn't already open
        public static void tryOpenPort()
        {          
            if (myPort.IsOpen == false)
            {
                try
                {
                    myPort.Open();
                }
                catch
                {
                    Console.WriteLine("Error: could not open serial port " + myPort.PortName + ".");
                    Console.ReadLine();
                }
            }
        }

        // Send string "toSend" over port "portName"
        public static void sendStringSerial(string toSend)
        {
            if (doSerial == true)
            {
                tryOpenPort();
                myPort.Write(toSend);
            }
        }

        // Start listening for data on port "portName"
        public static void startSerialListen()
        {
            if (doSerial == true)
            {
                tryOpenPort();
                myPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            }
        }

        // Runs when data received serially
        private static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataIn = myPort.ReadExisting();
            Console.WriteLine(dataIn);
        }

        public static void Main(string[] args)
        {            
            // Create serial port
            string portName = "COM3";
            myPort = new SerialPort(portName);

            // Try to send data over port
            string testString = "Hi there";
            sendStringSerial(testString);

            // Start receiving data over serial port (Arduino serial.write commands will show up)
            startSerialListen();

            // Get most recent CubeExplorer solution
            string webData = cubeExplorer.mostRecentSolution();

            // Display the solution received from CubeExplorer
            Console.WriteLine("Solution to carry out:");
            Console.WriteLine(webData + "\n");

            // Stores the best sequence of moves to carry out the solution from CubeExplorer discovered so far
            SolvingSequence bestSequence = Solver.findBestSeq(webData);

            // Prints the best sequence found
            Console.WriteLine("Best sequence found:");
            bestSequence.printSequence();
            Console.WriteLine();

            // Print statistics on the solving sequence
            Console.WriteLine("Best sequence statistics:");
            bestSequence.calculate();
            bestSequence.printStatistics();            

            // Print low level claw instructions to carry out bestSequence
            printLowLevel(bestSequence.moves);

            // Keeps the console window open
            Console.ReadLine();
        }

        // Writes a string both to the console and to the serial port
        // Commands are separated by spaces
        private static void displayAndSend(string toDisp)
        {
            Console.Write(toDisp + " ");
            sendStringSerial(toDisp + "  ");
        }

        // Sends low level commands for turning one face one time clockwise
        private static void turnOnceClockwise(string clawUsed)
        {
            displayAndSend(clawUsed);           // Turn the claw
            displayAndSend("open" + clawUsed);  // Open the claw
            displayAndSend(clawUsed + "p");     // Turn the claw back
            displayAndSend("close" + clawUsed); // Close the claw
        }

        // Sends low level commands for turning one face one time counter-clockwise
        private static void turnOnceCClockwise(string clawUsed)
        {
            displayAndSend(clawUsed + "p");     // Turn the claw
            displayAndSend("open" + clawUsed);  // Open the claw
            displayAndSend(clawUsed);           // Turn the claw back
            displayAndSend("close" + clawUsed); // Close the claw
        }

        // Handle the low level commands for the simple case in which we turn a face
        // (there is no cube rotation)
        // Turns cube faces only 90 degrees at a time
        private static void lowLevelTurnFace(string move)
        {           
            // Determine the claw used
            string clawUsed;
            if (move[0] == '1') // Left claw
            {
                clawUsed = "1";
            }
            else if (move[0] == '2') // Right claw
            {
                clawUsed = "2";
            }
            else
            {
                clawUsed = "error";
                Console.WriteLine("Invalid move in turnFace().");
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            // Turn a face once, clockwise
            if (move == clawUsed)
            {
                turnOnceClockwise(clawUsed);
            }
            // Turn a face once, counter-clockwise
            else if (move == (clawUsed + "p"))
            {
                turnOnceCClockwise(clawUsed);
            }
            // Turn a face twice
            else if (move == (clawUsed + "2"))
            {
                turnOnceClockwise(clawUsed);
                turnOnceClockwise(clawUsed);
            }
        }

        // Low level commands for SE cube rotation
        private static void lowLevelSE()
        {
            displayAndSend("open2");
            displayAndSend("1");
            displayAndSend("close2");
            displayAndSend("open1");
            displayAndSend("1p");
            displayAndSend("close1");  
        }

        // Low level commands for NE cube rotation
        private static void lowLevelNE()
        {
            displayAndSend("open1");
            displayAndSend("2");
            displayAndSend("close1");
            displayAndSend("open2");
            displayAndSend("2p");
            displayAndSend("close2");  
        }

        // Low level commands for SEp cube rotation
        private static void lowLevelSEp()
        {
            displayAndSend("open2");
            displayAndSend("1p");
            displayAndSend("close2");
            displayAndSend("open1");
            displayAndSend("1");
            displayAndSend("close1");  
        }

        // Low level commands for NEp cube rotation
        private static void lowLevelNEp()
        {
            displayAndSend("open1 ");
            displayAndSend("2p ");
            displayAndSend("close1 ");
            displayAndSend("open2 ");
            displayAndSend("2 ");
            displayAndSend("close2 "); 
        }

        // Low level printing code
        private static void printLowLevel(List<string> moves)
        { 
            for (int i = 0; i < moves.Count; i++)
            {                   
                // Turn a face
                if (moves[i].Contains("E") == false) // Not a SE or NE rotation command
                {
                    lowLevelTurnFace(moves[i]);
                }
                // Rotate the cube
                else
                {
                    if (moves[i] == "SE")
                    {
                        lowLevelSE();
                    }
                    else if (moves[i] == "NE")
                    {
                        lowLevelNE();
                    }
                    else if (moves[i] == "SEp")
                    {
                        lowLevelSEp();
                    }
                    else if (moves[i] == "NEp")
                    {
                        lowLevelNEp();
                    }
                    else if (moves[i] == "NE2")
                    {
                        lowLevelNE();
                        lowLevelNE();
                    }
                    else if (moves[i] == "SE2")
                    {
                        lowLevelSE();
                        lowLevelSE();
                    }
                    else
                    {
                        Console.WriteLine("Invalid move in printLowLevel().");
                        Console.ReadLine();
                        System.Environment.Exit(1);
                    }
                }
              
                Console.Write("\n");               
            }
        }
    }
}