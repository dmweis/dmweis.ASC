using dmweis.ASC.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndgleTurner
{
   class Program
   {
      static void Main( string[] args )
      {
         Console.WriteLine( "starting" );
         Arm arm = new Arm( "COM11", "default.xml" );
         arm.SetMagnet( false );
         while( true )
         {
            Console.WriteLine("Enter coord");
            string input = Console.ReadLine();
            string[] elements = input.Split( null );
            double x = double.Parse( elements[ 0 ] );
            double y = double.Parse( elements[ 1 ] );
            double z = double.Parse( elements[ 2 ] );
            arm.MoveToCartesian( x, y, z );
         }
         Console.WriteLine( "Press enter to leave" );
         Console.ReadLine();
      }
   }
}
