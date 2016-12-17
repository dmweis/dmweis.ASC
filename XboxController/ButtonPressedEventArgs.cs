using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XboxController
{
   public class ButtonPressedEventArgs
   {
      public string Button { get; set; }
      public bool IsPressed { get; set; }

      public ButtonPressedEventArgs( string button, bool isPressed )
      {
         Button = button;
         IsPressed = isPressed;
      }

      public override string ToString()
      {
         return IsPressed ? $"{Button} was pressed" : $"{Button} was released";
      }
   }
}
