using dmweis.ASC.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxController;

namespace AndgleTurner
{
   class Program
   {

      private static double baseAngle = 0.0;
      private static double distance = 10.0;
      private static double z = 0.0;
      private static bool magnetOn = false;

      static void Main( string[] args )
      {
         Console.WriteLine( "starting" );
         XboxControllerFactory xbox = new XboxControllerFactory();
         Arm arm = new Arm( "COM11", "direct.xml" );
         arm.SetMagnet( false );
         xbox.ControllerUpdate += ( sender, sticks ) =>
         {
            baseAngle += Math.Round( (sticks.LeftX - 50.0) / 39.0 ) * 2;
            distance += Math.Round( (sticks.LeftY - 50.0) / 39.0 ) * 0.5;
            z += Math.Round( (sticks.RightY - 50.0) / 39.0 ) * 0.5;
            arm.MoveTo2DCartesianAsync( baseAngle, distance, z ).Wait();
         };
         xbox.ButtonPressed += ( sender, buttonPress ) =>
         {
            if( buttonPress.IsPressed && buttonPress.Button == "A" )
            {
               magnetOn = !magnetOn;
               arm.SetMagnet( magnetOn );
            }
         };
         while( true )
         {
            Console.WriteLine("Enter coord");
            string input = Console.ReadLine();
            if( input.ToLowerInvariant().Contains("on") )
            {
               arm.SetMagnet( true ).Wait();
               continue;
            }
            if( input.ToLowerInvariant().Contains( "off" ) )
            {
               arm.SetMagnet( false ).Wait();
               continue;
            }
            string[] elements = input.Split( null );
            double x = double.Parse( elements[ 0 ] );
            double y = double.Parse( elements[ 1 ] );
            double z = double.Parse( elements[ 2 ] );
            arm.MoveToCartesianAsync( x, y, z ).Wait();
         }
         Console.WriteLine( "Press enter to leave" );
         Console.ReadLine();
      }
   }
}
