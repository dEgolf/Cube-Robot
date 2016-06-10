using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    static class Solver
    {
        // Find the best way to carry out the solution "inSoln" using our claws
        public static SolvingSequence findBestSeq(string inSoln)
        {
            SolvingSequence bestSequence = new SolvingSequence(new List<string> { });
            bestSequence.totalTime = 9000; // The total time of this sequence is initialized to a very high value so that the first sequence found will replace this empty starting one

            // The program loops a certain number of times. Each time, it finds a sequence of moves that will carry out the solution given by CubeExplorer.
            // The best solution is stored and displayed at the end.
            for (int j = 0; j < 1000; j++)
            {
                // For debugging purposes, one possible color arrangement is as follows: Red = front, green = right, yellow = up, orange = back, blue = left, white = down.
                // Using colors makes it easier to keep track which face is the F face, for instance, after the cube has been rotated.
                // This constructor takes the three cycles produced by the NE, SE, and equatorial rotations
                Cube myCube = new Cube(new List<string> { "F", "D", "B", "U" }, new List<string> { "R", "U", "L", "D" }, new List<string> { "R", "B", "L", "F" });

                // Creates a SolvingSequence to hold the sequence of moves that will be generated
                SolvingSequence sequence = new SolvingSequence(new List<string> { });

                // Goes through each instruction the solving method provided by CubeExplorer, and determines a way in which the robot could carry it out
                for (int i = 0; i < inSoln.Length; i++)
                {
                    // Gets the next character in the instructions.
                    // This can be the next face to access, but also spaces and modifiers like "2" and "'"
                    // We only work with it if it is a face, and we look ahead to see modifiers
                    char face = inSoln[i];

                    // If the next character specifies a face to be accessed, this section stores a move to access the face in sequence.moves
                    if (face == 'R' || face == 'L' || face == 'F' || face == 'U' || face == 'B' || face == 'D')
                    {
                        // Determines the needed cube rotation, if any
                        string rotationUsed = myCube.AccessFace(Convert.ToString(face));

                        // Stores the cube rotation, if applicable
                        if (rotationUsed != "")
                        {
                            sequence.moves.Add(rotationUsed);
                        }

                        // Determines and stores the face that needs to be turned
                        if (myCube.ne.faceList[0] == Convert.ToString(face))
                        {
                            if (inSoln[i + 1] == ' ')
                            {
                                sequence.moves.Add("1");
                            }
                            else if (inSoln[i + 1] == '\'')
                            {
                                sequence.moves.Add("1p");
                            }
                            else if (inSoln[i + 1] == '2')
                            {
                                sequence.moves.Add("12");
                            }
                        }
                        else if (myCube.se.faceList[0] == Convert.ToString(face))
                        {
                            if (inSoln[i + 1] == ' ')
                            {
                                sequence.moves.Add("2");
                            }
                            else if (inSoln[i + 1] == '\'')
                            {
                                sequence.moves.Add("2p");
                            }
                            else if (inSoln[i + 1] == '2')
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
                    
                    bool verboseSeqResults = false;
                    if (verboseSeqResults == true)
                    {
                        bestSequence.printSequence();
                        bestSequence.calculate();
                        bestSequence.printStatistics();
                    }
                }
            }

            return bestSequence;
        }
    }
}
