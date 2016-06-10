using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    static class cubeExplorer
    {
        // Get the most recent solution from CubeExplorer
        // Good to use for debugging purposes when you don't want to scan a real cube
        public static string mostRecentSolution()
        {

            // For debugging purposes, the most recent solution from CubeExplorer is used.
            // In reality, ScandAndSolve() should be used to get the solution for the desired cube.
            System.Net.WebClient wc = new System.Net.WebClient();
            string webData = "";
            try
            {
                webData = wc.DownloadString("http://127.0.0.1:8081/?getLast");
            }
            // If CubeExplorer is not open...
            catch
            {
                Console.WriteLine("Please open CubeExplorer.");
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            // Removes unneeded characters from the solution string received from CubeExplorer
            webData = trimText(webData);

            // Gives an error message and closes if CubeExplorer does not send a solution to a cube
            if (webData == "\r\nBuffer is empty!\r\n" || webData == "\r\ninvalid request\r\n")
            {
                Console.WriteLine("Please enter a cube to be solved.");
                Console.ReadLine();
                System.Environment.Exit(1);
            }
            return webData;
        }

        // Removes unneeded characters from the text received from CubeExplorer
        private static string trimText(string toTrim)
        {
            toTrim = toTrim.Remove(0, 12);
            toTrim = toTrim.Substring(0, toTrim.LastIndexOf("</BODY"));

            toTrim = toTrim.Remove(0, 2);
            toTrim = toTrim.TrimEnd('\n');
            toTrim = toTrim.TrimEnd('\r');
            return toTrim;
        }

        // Coordinates the scanning of the cube and receives the solution from CubeExplorer.
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

    }
}
