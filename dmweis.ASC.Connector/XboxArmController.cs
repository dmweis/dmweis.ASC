using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using XboxController;

namespace dmweis.ASC.Connector
{
   public class XboxArmController
   {
      private XboxControllerFactory m_ControllerFactory;
      private Arm m_Arm;
      private bool m_MagnetOn;

      public XboxArmController( XboxControllerFactory controllerFactory, Arm arm )
      {
         m_ControllerFactory = controllerFactory;
         m_Arm = arm;
         m_ControllerFactory.ButtonPressed += ButtonStateChange;
      }

      private void ButtonStateChange( object sender, ButtonPressedEventArgs e )
      {
         if( e.IsPressed && e.Button == "A" )
         {
            if( m_MagnetOn )
            {
               m_MagnetOn = false;
               m_Arm.SetMagnet( false );
            }
            else
            {
               m_MagnetOn = true;
               m_Arm.SetMagnet( true );
            }
         }
      }
   }
}
