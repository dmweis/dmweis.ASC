using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxController;

namespace XboxControllerTestConsole
{
   class Program
   {
      static void Main( string[] args )
      {
         Console.WriteLine( "Starting" );
         XboxControllerFactory listener = new XboxControllerFactory();
         listener.ControllerChanged += Listener_ControllerChanged;
         listener.ButtonPressed += Listener_ButtonPressed;
         Console.WriteLine( "Done" );
         Console.ReadLine();
      }

      private static void Listener_ButtonPressed( object sender, ButtonPressedEventArgs e )
      {
         Console.WriteLine( e.ToString() );
      }

      private static void Listener_ControllerChanged( object sender, StickValues e )
      {
         Console.WriteLine( e.ToString() );
      }
   }
}
