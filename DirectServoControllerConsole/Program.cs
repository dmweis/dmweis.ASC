using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using dmweis.ASC.Connector;

namespace DirectServoControllerConsole
{
   class Program
   {
      static void Main( string[] args )
      {
         Console.WriteLine("Starting");
         ArmBase arm = new Arm("COM11", "direct.xml");
         while (true)
         {
            Console.WriteLine("Enter coordiantes");
            string[] elements = Console.ReadLine().Split( (char[]) null, StringSplitOptions.RemoveEmptyEntries );
            double @base = double.Parse(elements[0]);
            double shoulder = double.Parse(elements[1]);
            double elbow = double.Parse(elements[2]);
            Console.WriteLine( $"Moving to Base: {@base} Shoulder: {shoulder} Elbow: {elbow}");
            arm.MoveServosToAsync(@base, shoulder, elbow).Wait();
         }
      }
   }
}
