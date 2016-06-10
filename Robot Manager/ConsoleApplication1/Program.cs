using System;
using System.Collections.Generic;
using System.Diagnostics;  // To use Process.Start
using System.IO.Ports;

namespace ConsoleApplication1
{
    internal class Program
    {
        static SerialPort myPort;

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
            tryOpenPort();
            myPort.Write(toSend);
        }

        // Start listening for data on port "portName"
        public static void startSerialListen()
        {
            tryOpenPort();
            myPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
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
            // startSerialListen();

            // For debugging purposes, the most recent solution from CubeExplorer is used.
            // In reality, ScandAndSolve() should be used to get the solution for the desired cube.
            System.Net.WebClient wc = new System.Net.WebClient();
            string webData = "";
            try
            {
                webData = wc.DownloadString("http://127.0.0.1:8081/?getLast");
            }
            // Errors if cube explorer is not open
            catch
            {    
                Console.WriteLine("Please open CubeExplorer.");
                Console.ReadLine();
                return;                
            }

            // Removes unneeded characters from the solution string received from CubeExplorer
            webData = trimText(webData);
            
            // Gives an error message and closes if CubeExplorer does not send a solution to a cube
            if (webData == "\r\nBuffer is empty!\r\n")
            {
                Console.WriteLine("Please enter a cube to be solved.");
                Console.ReadLine();
                return;
            }

            // Displays the solution received from CubeExplorer
            Console.WriteLine(webData);

            // Stores the best sequence of moves to carry out the solution from CubeExplorer discovered so far
            SolvingSequence bestSequence = new SolvingSequence(new List<string> { });
            bestSequence.totalTime = 9000; // The total time of this sequence is initialized to a very high value so that the first sequence found will replace this empty starting one

            // The program loops a certain number of times. Each time, it finds a sequence of moves that will carry out the solution given by CubeExplorer.
            // The best solution is stored and displayed at the end.
            for (int j = 0; j < 1000; j++)
            {
                // For debugging purposes, one possible color arrangement is as follows: Red = front, green = right, yellow = up, orange = back, blue = left, white = down.
                // Using colors makes it easier to keep track which face is the F face, for instance, after the cube has been rotated.
                Cube testCube = new Cube(new List<string> { "F", "D", "B", "U" }, new List<string> { "R", "U", "L", "D" }, new List<string> { "R", "B", "L", "F" });

                // Creates a SolvingSequence to hold the sequence of moves that will be generated
                SolvingSequence sequence = new SolvingSequence(new List<string> { });

                // Goes through each instruction the solving method provided by CubeExplorer, and determines a way in which the robot could carry it out
                for (int i = 0; i < webData.Length; i++)
                {
                    // Gets the next character in the instructions.
                    char face = webData[i];

                    // If the next character specifies a face to be accessed, this section stores a method to access the face in sequence.moves
                    if (face == 'R' || face == 'L' || face == 'F' || face == 'U' || face == 'B' || face == 'D')
                    {
                        // Determines the needed cube rotation, if any
                        string rotationUsed = testCube.AccessFace(Convert.ToString(face));

                        // Stores the cube rotation, if applicable
                        if (rotationUsed != "")
                        {
                            sequence.moves.Add(rotationUsed);
                        }

                        // Determines and stores the face that needs to be turned
                        if (testCube.ne.faceList[0] == Convert.ToString(face))
                        {
                            if (webData[i + 1] == ' ')
                            {
                                sequence.moves.Add("1");
                            }
                            else if (webData[i + 1] == '\'')
                            {
                                sequence.moves.Add("1p");
                            }
                            else if (webData[i + 1] == '2')
                            {
                                sequence.moves.Add("12");
                            }
                        }
                        else if (testCube.se.faceList[0] == Convert.ToString(face))
                        {
                            if (webData[i + 1] == ' ')
                            {
                                sequence.moves.Add("2");
                            }
                            else if (webData[i + 1] == '\'')
                            {
                                sequence.moves.Add("2p");
                            }
                            else if (webData[i + 1] == '2')
                            {
                                sequence.moves.Add("22");
                            }
                        }
                    }
                }

                // Calculates how long the sequence takes to execute
                sequence.calculate();

                // Stores the sequence just calculated in bestSequence if the just calculated sequence is faster than the best sequence found so far
                if (sequence.totalTime < bestSequence.totalTime)
                {
                    bestSequence.totalTime = sequence.totalTime;
                    bestSequence.moves.Clear();

                    for (int i = 0; i < sequence.moves.Count; i++)
                    {
                        bestSequence.moves.Add(sequence.moves[i]);
                    }
                }
            }

            // Prints the best sequence found, along with the time it takes to execute
            Console.WriteLine("Best sequence found:");
            bestSequence.calculate();
            bestSequence.printStatistics();
            bestSequence.printSequence();


            // Print low level claw instructions to carry out bestSequence
            Console.WriteLine();
            printLowLevel(bestSequence.moves);

            // Keeps the console window open
            Console.ReadLine();
        }

        // Manages the interactions with CubeExplorer. Namely, it coordinates the scanning of the cube and receives the solution from CubeExplorer.
        public static string ScanAndSolve()
        {
            // wc is used to interact with CubeExplorer
            System.Net.WebClient wc = new System.Net.WebClient();

            // The scanning has to be done in the order B,L,F,R,U,D
            // The user needs to initialize CubeExplorer by giving it several red and orange color samples

            wc.DownloadString("http://127.0.0.1:8081/?scanB");

            // Tell the robot to turn cube
            Console.ReadLine(); // placeholder for debugging purposes

            wc.DownloadString("http://127.0.0.1:8081/?scanL");

            // Tell the robot to turn cube
            Console.ReadLine(); // placeholder for debugging purposes

            wc.DownloadString("http://127.0.0.1:8081/?scanF");

            // Tell the robot to turn cube
            Console.ReadLine(); // placeholder for debugging purposes

            wc.DownloadString("http://127.0.0.1:8081/?scanR");

            // Tell the robot to turn cube
            Console.ReadLine(); // placeholder for debugging purposes

            wc.DownloadString("http://127.0.0.1:8081/?scanU");

            // Tell the robot to turn cube
            Console.ReadLine(); // placeholder for debugging purposes

            wc.DownloadString("http://127.0.0.1:8081/?scanD");

            // Gets the solution from Cube Explorer
            wc.DownloadString("http://127.0.0.1:8081/?transfer");

            // Gets the most recent solution
            string webData = wc.DownloadString("http://127.0.0.1:8081/?getLast");

            // Removes unneeded character from the start and end of the string
            webData = webData.Remove(0, 12);
            webData = webData.Substring(0, webData.LastIndexOf("</BODY"));

            // Displays the solution
            Console.WriteLine(webData);

            return webData;
        }

        // Removes unneeded characters from the text received from CubeExplorer
        private static string trimText(string toTrim)
        {
            toTrim = toTrim.Remove(0, 12);
            toTrim = toTrim.Substring(0, toTrim.LastIndexOf("</BODY"));
            return toTrim;
        }


        // Low level printing code
        private static void printLowLevel(List<string> moves)
        {
            List<string> usefulClawOpenClose = new List<string>();

            for (int i = 0; i < moves.Count; i++)
            {
                Console.Write("(");

                // Dealing not with the cube rotations
                if (moves[i].Contains("E") == false) // Not a NE or SE rotation
                {
                    Console.Write(moves[i] + " ");

                    Console.Write("open" + moves[i][0] + " ");

                    if (moves[i] == "1")
                    {
                        Console.Write("1p");

                        if (i < moves.Count - 1)
                        {
                            if (moves[i + 1][0] != 'N')
                            {
                                Console.Write(" close" + moves[i]);
                                usefulClawOpenClose.Add("c" + moves[i][0]);
                            }
                            else
                            {
                                usefulClawOpenClose.Add("S");
                                Console.Write(" SKIP");
                            }
                        }
                    }
                    else if (moves[i] == "2")
                    {
                        Console.Write("2p");

                        if (i < moves.Count - 1)
                        {
                            if (moves[i + 1][0] != 'S')
                            {
                                Console.Write(" close" + moves[i]);
                                usefulClawOpenClose.Add("c" + moves[i][0]);
                            }
                            else
                            {
                                usefulClawOpenClose.Add("S");
                                Console.Write(" SKIP");
                            }
                        }
                    }
                    else if (moves[i] == "2p")
                    {
                        Console.Write("2");

                        if (i < moves.Count - 1)
                        {
                            if (moves[i + 1][0] != 'S')
                            {
                                Console.Write(" close" + moves[i]);
                                usefulClawOpenClose.Add("c" + moves[i][0]);
                            }
                            else
                            {
                                usefulClawOpenClose.Add("S");
                                Console.Write(" SKIP");
                            }
                        }
                    }
                    else if (moves[i] == "1p")
                    {
                        Console.Write("1");

                        if (i < moves.Count - 1)
                        {
                            if (moves[i + 1][0] != 'N')
                            {
                                Console.Write(" close" + moves[i]);
                                usefulClawOpenClose.Add("c" + moves[i][0]);
                            }
                            else
                            {
                                usefulClawOpenClose.Add("S");
                                Console.Write(" SKIP");
                            }
                        }
                    }
                    else if (moves[i] == "12")
                    {
                        Console.Write("12");

                        if (i < moves.Count - 1)
                        {
                            if (moves[i + 1][0] != 'N')
                            {
                                Console.Write(" close" + moves[i][0]);
                                usefulClawOpenClose.Add("c" + moves[i][0]);
                            }
                            else
                            {
                                usefulClawOpenClose.Add("S");
                                Console.Write(" SKIP");
                            }
                        }
                    }
                    else //  (moves[i] == "22")
                    {
                        Console.Write("22");

                        if (i < moves.Count - 1)
                        {
                            if (moves[i + 1][0] != 'N')
                            {
                                Console.Write(" close" + moves[i][0]);
                                usefulClawOpenClose.Add("c" + moves[i][0]);
                            }
                            else
                            {
                                usefulClawOpenClose.Add("S");
                                Console.Write(" SKIP");
                            }
                        }
                    }

                }
                else // dealing with the cube rotations
                {
                    if (moves[i] == "SE")
                    {
                        usefulClawOpenClose.Add("o2");
                        Console.Write("open2 ");
                        Console.Write("1 ");
                        Console.Write("close2 ");
                        Console.Write("open1 ");
                        Console.Write("1p ");
                        Console.Write("close1 ");
                        usefulClawOpenClose.Add("c1");
                    }
                    else if (moves[i] == "NE")
                    {
                        usefulClawOpenClose.Add("o1");
                        Console.Write("open1 ");
                        Console.Write("2 ");
                        Console.Write("close1 ");
                        Console.Write("open2 ");
                        Console.Write("2p ");
                        Console.Write("close2 ");
                        usefulClawOpenClose.Add("c2");
                    }
                    else if (moves[i] == "SEp")
                    {
                        usefulClawOpenClose.Add("o2");
                        Console.Write("open2 ");
                        Console.Write("1p ");
                        Console.Write("close2 ");
                        Console.Write("open1 ");
                        Console.Write("1 ");
                        Console.Write("close1 ");
                        usefulClawOpenClose.Add("c1");
                    }
                    else if (moves[i] == "NEp")
                    {
                        usefulClawOpenClose.Add("o1");
                        Console.Write("open1 ");
                        Console.Write("2p ");
                        Console.Write("close1 ");
                        Console.Write("open2 ");
                        Console.Write("2 ");
                        Console.Write("close2 ");
                        usefulClawOpenClose.Add("c2");
                    }
                    else if (moves[i] == "NE2")
                    {
                        usefulClawOpenClose.Add("o1");
                        Console.Write("open1 ");
                        Console.Write("22 ");
                        Console.Write("close1 ");
                        Console.Write("open2 ");
                        Console.Write("22 ");
                        Console.Write("close2 ");
                        usefulClawOpenClose.Add("c2");
                    }
                    else // move == SE2
                    {
                        usefulClawOpenClose.Add("o2");
                        Console.Write("open2 ");
                        Console.Write("12 ");
                        Console.Write("close2 ");
                        Console.Write("open1 ");
                        Console.Write("12 ");
                        Console.Write("close1 ");
                        usefulClawOpenClose.Add("c1");
                    }
                }

                Console.Write(")");
                Console.WriteLine();

               
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