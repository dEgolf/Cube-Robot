using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // For debugging purposes, the most recent solution from CubeExplorer is used.
            // In reality, ScandAndSolve() should be used to get the solution for the desired cube.
            System.Net.WebClient wc = new System.Net.WebClient();
            string webData = "";
            try
            {
                webData = wc.DownloadString("http://127.0.0.1:8081/?getLast");
            }
            // Gives an error message and closes if CubeExplorer is not open
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
    }
}