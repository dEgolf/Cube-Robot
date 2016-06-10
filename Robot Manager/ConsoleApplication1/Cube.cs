using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    // Holds the current state of the cube
    public class Cube
    {
        // The cube is completely described by these three cycles.
        // See Cycle.cs for a discussion of cycles.

        public Cycle ne; // The NE cycle
        public Cycle se; // The SE cycle
        public Cycle v; // The V cycle

        private static Random myRand = new Random();

        public Cube(List<string> neFaces, List<string> seFaces, List<string> vFaces)
        {
            ne = new Cycle(neFaces);
            se = new Cycle(seFaces);
            v = new Cycle(vFaces);
        }

        // Returns, in the form of a string, the cube rotation necessary to access the inputted face
        public string AccessFace(string face)
        {
            // In this case, the face is already accessible (the start of the face-lists holds the immediately accessible faces)
            if (face == ne.faceList[0] || face == se.faceList[0])
            {
                return "";
            }

            // The face can be accessed using a single NE rotation or a SE prime rotation
            else if (face == ne.faceList[1])
            {
                // The program randomly decides which way to access the face
                if (makeChoice() == 0)
                {
                    CubeNE();
                    return "NE";
                }
                else
                {
                    CubeSEPrime();
                    return "SEp";
                }
            }

            // The face can be accessed using a single SE rotation or a NE prime rotation
            else if (face == se.faceList[1]) 
            {
                // The program randomly decides which way to access the face
                if (makeChoice() == 0)
                {
                    CubeSE();
                    return "SE";
                }
                else
                {
                    CubeNEPrime();
                    return "NEp";
                }
            }

            // The face can be accessed using a single NE' rotation
            else if (face == ne.faceList[3])
            {
                CubeNEPrime();
                return "NEp";
            }

            // The face can be accessed using a single SE' rotation
            else if (face == se.faceList[3])
            {
                CubeSEPrime();
                return "SEp";
            }

            // The face can be accessed using a NE2 rotation
            else if (face == ne.faceList[2])
            {
                CubeNE2();
                return "NE2";
            }

            // The face can be accessed using a SE2 rotation
            else if (face == se.faceList[2])
            {
                CubeSE2();
                return "SE2";
            }

            // The program should never reach this line. If it does, an error message is displayed and the program pauses.
            Console.WriteLine("Error in AccessFace!");
            Console.ReadLine();

            return "ERROR";
        }

        // Carries out the NE cube rotation. This rotation changes the cycles of the cube.
        public void CubeNE()
        {
            // temp = SE
            Cycle temp = new Cycle(new List<string> { "A", "B", "C", "D" });
            copyFaceList(temp, se);

            // SE  = V flip
            copyFaceList(se, v);
            se.flip();

            // NE = NE shift
            ne.shift();

            // V = temp (temp is SE)
            copyFaceList(v, temp);
        }

        // Carries out the NE2 cube rotation.
        public void CubeNE2()
        {
            CubeNE();
            CubeNE();
        }

        // Carries out the NE prime cube rotation.
        public void CubeNEPrime()
        {
            CubeNE();
            CubeNE();
            CubeNE();
        }

        // Carries out the SE cube rotation.
        public void CubeSE()
        {
            // temp = NE
            Cycle temp = new Cycle(new List<string> { "A", "B", "C", "D" });
            copyFaceList(temp, ne);

            // NE = V inverse shift
            copyFaceList(ne, v);
            ne.inverseShift();

            // SE = SE shift
            se.shift();

            // V = NE reverse (temp is NE)
            copyFaceList(v, temp);
            v.reverse();
        }

        // Carries out the SE2 cube rotation.
        public void CubeSE2()
        {
            CubeSE();
            CubeSE();
        }

        // Carries out the SE prime cube rotation.
        public void CubeSEPrime()
        {
            CubeSE();
            CubeSE();
            CubeSE();
        }

        // Prints the cycles of the cube. Used for debugging purposes.
        public void printCube()
        {
            Console.Write("NE Cycle: "); ne.print(); Console.WriteLine();
            Console.Write("SE Cycle: "); se.print(); Console.WriteLine();
            Console.Write("V Cycle: "); v.print(); Console.WriteLine();
        }

        // Copies "From" to "To"
        private void copyFaceList(Cycle To, Cycle From)
        {
            for (int i = 0; i < 4; i++)
            {
                To.faceList[i] = From.faceList[i];
            }
        }

        // Returns randomly either a 0 or a 1
        private int makeChoice()
        {
            int choice = myRand.Next(0, 2);
            return choice;
        }
    }
}