using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    // Each cube configuration has three cycles.
    // The NE cycle is a list of faces that can be accessed (by the left claw) by performing a NE cube rotation repeatedly (rotate TO the NE - right claw turns clockwise)
    // -The faces are listed in this order, if the cube is at its starting position: F, D, B, U
    // The SE cycle is a list of faces that can be accessed (by the right claw) by performing a SE cube rotation repeatedly (rotate TO the SE - left claw turns clockwise)
    // -The faces are listed in this order, if the cube is at its starting position: R, U, L, D
    // The V cycle is the list of faces that excludes the up and down face (go around the equator)
    

    // Rotating the cube changes which faces (relative to original orientation) are in which cycle
    // Each of the methods below are used as building blocks to deal with these changes
    public class Cycle
    {
        // Holds the list of faces in this cycle
        public List<string> faceList;

        public Cycle(List<string> faces)
        {
            faceList = faces;
        }

        // flip(), inverseShift(), reverse(), and shift() are all transformations that reorder the faces in a cycle

        // A B C D -> A D C B
        public void flip()
        {
            string temp = faceList[3];
            faceList[3] = faceList[1];
            faceList[1] = temp;
        }

        // A B C D -> D A B C
        public void inverseShift()
        {
            string temp = faceList[0];
            faceList[0] = faceList[3];
            faceList[3] = faceList[2];
            faceList[2] = faceList[1];
            faceList[1] = temp;
        }

        // A B C D -> D C B A
        public void reverse()
        {
            string temp = faceList[0];
            string temp2 = faceList[1];
            faceList[0] = faceList[3];
            faceList[1] = faceList[2];
            faceList[2] = temp2;
            faceList[3] = temp;
        }

        // A B C D -> B C D A
        public void shift()
        {
            string temp = faceList[0];
            faceList[0] = faceList[1];
            faceList[1] = faceList[2];
            faceList[2] = faceList[3];
            faceList[3] = temp;
        }

        // Prints the faces in this cycle in order
        public void print()
        {
            foreach (string face in faceList)
            {
                Console.Write(face + " ");
            }
        }
    }
}