using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    internal class SolvingSequence
    {
        // Stores the list of moves that are contained in this sequence (along with statistic on its composition)
        // Here is a legend:
        // NE stands for the NE cube rotation.
        // --This means we rotate the cube TO the north east (the right claw turns clockwise, and the cube turns with it)
        // -The 2 suffix means to carry out that move twice.
        // -The p suffix means to carry out the inverse of that move.
        // -If a move starts with "1", then that means "rotate claw 1", that is, the claw on the left (clockwise)
        // -If a move starts with "2", then that means "rotate claw 2", that is, the claw on the right (clockwise)
        // List of moves used: NE2, SE2, NE, SE, NEp, SEp,1,2,12,22,1p,2p
        public List<string> moves;

        private int totalQuarterClawRotations = 0;
        private int totalTimesClawOpenedAndClosed = 0;  // Opening and closing a claw once would only add ONE to this variable
        private double timeClawOpen = 0.5;              // The time in seconds that it takes for the robot to open a claw
        private double timePerClawTurn = 1.0;           // The time in seconds that it takes for the robot to turn a claw 1/4 of a full rotation

        private int doubleCubeRotations = 0;    // Double cube rotations are: NE2 and SE2
        private int singleCubeRotations = 0;    // Single cube rotations are : NE, SE, NEp, SEp
        private int totalCubeRotations = 0;     // The number of quarter turns the cube is rotated
        private int totalFaceRotations = 0;     // The total number of quarter turn face rotations performed

        // The total time to execute the sequence
        public double totalTime = 0;

        // The constructor takes a sequence of moves to be executed and stores them
        public SolvingSequence(List<string> inMoves)
        {
            moves = inMoves;
        }

        // Prints each move in the sequence
        public void printSequence()
        {
            foreach (string move in moves)
            {
                Console.Write(move + " ");
            }
            Console.WriteLine();
        }

        // Displays the statistics calculated in calculate()
        public void printStatistics()
        {
            Console.WriteLine("Total quarter cube rotations: " + totalCubeRotations);
            Console.WriteLine("Total quarter face rotations: " + totalFaceRotations);
            Console.WriteLine("Total quarter claw rotations: " + totalQuarterClawRotations);
            Console.WriteLine("Total times claw opened and closed: " + totalTimesClawOpenedAndClosed);
            Console.Write("Total time: "); writeAsMinuteSecond(totalTime);
            Console.WriteLine();
        }

        // Calculates various statistics, including the total time required to execute the sequence
        public void calculate()
        {
            // Searches through the move sequence and counts how many of various types of cube rotations occur
            doubleCubeRotations = moves.FindAll(element => element == "NE2" || element == "SE2").Count;

            singleCubeRotations = moves.FindAll(element => element == "SE" || element == "NE" || element == "NEp" || element == "SEp").Count;

            totalCubeRotations = doubleCubeRotations * 2 + singleCubeRotations;

            totalFaceRotations = moves.FindAll(element => element == "1" || element == "2" || element == "1p" || element == "2p").Count +
                moves.FindAll(element => element == "12" || element == "22").Count * 2;

            // Calculates the number of times a claw is turned 1/4 turn
            totalQuarterClawRotations = 2 * totalFaceRotations + 2 * totalCubeRotations;

            // Calculates the number of times a claw is opened and closed
            totalTimesClawOpenedAndClosed = totalFaceRotations + 2 * (doubleCubeRotations + singleCubeRotations);

            // Calculates the total time to carry out the solution
            totalTime = totalQuarterClawRotations * timePerClawTurn + totalTimesClawOpenedAndClosed * timeClawOpen * 2;
        }

        // Writes a time in seconds in minutes and seconds. For example, 71 seconds is write as 1 minute(s) and 11 seconds.
        private static void writeAsMinuteSecond(double time)
        {
            Console.WriteLine((int)time / 60 + " minute(s) and "
                     + (int)(60 * ((double)(time / 60) - (int)(time / 60))) + " s.");
        }


    }
}