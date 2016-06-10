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
        private static void turnFace(string move)
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


        // Low level printing code
        private static void printLowLevel(List<string> moves)
        {
            List<string> usefulClawOpenClose = new List<string>();

            for (int i = 0; i < moves.Count; i++)
            {                

                // Dealing not with the cube rotations (not a NE or SE rotation)
                // This means we need to turn a face
                if (moves[i].Contains("E") == false) // 
                {
                    turnFace(moves[i]);
                }

                else // dealing with the cube rotations
                {
                    if (moves[i] == "SE")
                    {
                        usefulClawOpenClose.Add("o2");
                        displayAndSend("open2 ");
                        displayAndSend("1 ");
                        displayAndSend("close2 ");
                        displayAndSend("open1 ");
                        displayAndSend("1p ");
                        displayAndSend("close1 ");
                        usefulClawOpenClose.Add("c1");
                    }
                    else if (moves[i] == "NE")
                    {
                        usefulClawOpenClose.Add("o1");
                        displayAndSend("open1 ");
                        displayAndSend("2 ");
                        displayAndSend("close1 ");
                        displayAndSend("open2 ");
                        displayAndSend("2p ");
                        displayAndSend("close2 ");
                        usefulClawOpenClose.Add("c2");
                    }
                    else if (moves[i] == "SEp")
                    {
                        usefulClawOpenClose.Add("o2");
                        displayAndSend("open2 ");
                        displayAndSend("1p ");
                        displayAndSend("close2 ");
                        displayAndSend("open1 ");
                        displayAndSend("1 ");
                        displayAndSend("close1 ");
                        usefulClawOpenClose.Add("c1");
                    }
                    else if (moves[i] == "NEp")
                    {
                        usefulClawOpenClose.Add("o1");
                        displayAndSend("open1 ");
                        displayAndSend("2p ");
                        displayAndSend("close1 ");
                        displayAndSend("open2 ");
                        displayAndSend("2 ");
                        displayAndSend("close2 ");
                        usefulClawOpenClose.Add("c2");
                    }
                    else if (moves[i] == "NE2")
                    {
                        usefulClawOpenClose.Add("o1");
                        displayAndSend("open1 ");

                        displayAndSend("22 ");

                        displayAndSend("close1 ");
                        displayAndSend("open2 ");

                        displayAndSend("22 ");

                        displayAndSend("close2 ");
                        usefulClawOpenClose.Add("c2");
                    }
                    else // move == SE2
                    {
                        usefulClawOpenClose.Add("o2");
                        displayAndSend("open2 ");

                        displayAndSend("12 ");

                        displayAndSend("close2 ");
                        displayAndSend("open1 ");

                        displayAndSend("12 ");

                        displayAndSend("close1 ");
                        usefulClawOpenClose.Add("c1");
                    }
                }
              
                Console.Write("\n");               
            }

            //// Weed out useless stuff like o1 skip o1
            //for (int i =  usefulClawOpenClose.Count - 2; i >= 0; i--)
            //{
            //    if (usefulClawOpenClose[i] == "S")
            //    {
            //        usefulClawOpenClose[i + 1] = "S";
            //    }
            //}

            //Console.WriteLine();
            //foreach (string openClose in usefulClawOpenClose)
            //{
            //    Console.WriteLine(openClose);
            //}
           
        }
    }
}