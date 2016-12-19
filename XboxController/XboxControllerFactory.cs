using BrandonPotter.XBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XboxController
{
    public class XboxControllerFactory
    {
      private Task m_ControllerListenerTask;
      private CancellationTokenSource m_CancellationTokenSource;

      private Dictionary<string, bool> m_ButtonStates;
      private StickValues m_LastValues;
      private int m_LastUpdate;

      public int UpdateFrequency { get; set; }

      public event EventHandler<StickValues> ControllerChanged;
      public event EventHandler<StickValues> ControllerUpdate;
      public event EventHandler<ButtonPressedEventArgs> ButtonPressed;

      public XboxControllerFactory()
      {
         UpdateFrequency = 50;
         m_CancellationTokenSource = new CancellationTokenSource();
         m_ButtonStates = new Dictionary<string, bool>();
         m_LastValues = new StickValues();
         m_ControllerListenerTask = Task.Factory.StartNew( ControllerListenerLoopAsync, TaskCreationOptions.LongRunning );
      }

      private async void ControllerListenerLoopAsync()
      {
         CancellationToken cancellationTokenSource = m_CancellationTokenSource.Token;
         XBoxController connectedController = XBoxController.GetConnectedControllers().FirstOrDefault();
         while( !cancellationTokenSource.IsCancellationRequested )
         {
            await Task.Delay( 20 );
            if( connectedController.IsConnected )
            {
               StickValues newValues = new StickValues( connectedController.ThumbLeftX, connectedController.ThumbLeftY, connectedController.ThumbRightX, connectedController.ThumbRightY );
               if( m_LastUpdate < Environment.TickCount )
               {
                  m_LastUpdate = Environment.TickCount + UpdateFrequency;
                  ControllerUpdate?.Invoke( this, newValues );
               }
               if( newValues != m_LastValues )
               {
                  m_LastValues = newValues;
                  ControllerChanged?.Invoke( this, newValues );
               }
               CheckButton( "A", connectedController.ButtonAPressed );
               CheckButton( "B", connectedController.ButtonBPressed );
               CheckButton( "X", connectedController.ButtonXPressed );
               CheckButton( "Y", connectedController.ButtonYPressed );
               CheckButton( "Back", connectedController.ButtonBackPressed );
               CheckButton( "Start", connectedController.ButtonStartPressed );
               CheckButton( "Up", connectedController.ButtonUpPressed );
               CheckButton( "Down", connectedController.ButtonDownPressed );
               CheckButton( "Left", connectedController.ButtonLeftPressed );
               CheckButton( "Right", connectedController.ButtonRightPressed );
               CheckButton( "LB", connectedController.ButtonShoulderLeftPressed );
               CheckButton( "RB", connectedController.ButtonShoulderRightPressed );
               CheckButton( "LT", connectedController.TriggerLeftPressed );
               CheckButton( "RT", connectedController.TriggerRightPressed );
               CheckButton( "LS", connectedController.ThumbpadLeftPressed );
               CheckButton( "RS", connectedController.ThumbpadRightPressed );
            }
         }
      }

      private void CheckButton( string button, bool isPressed )
      {
         if( !m_ButtonStates.ContainsKey( button ) )
         {
            m_ButtonStates.Add( button, false );
         }
         if( m_ButtonStates[ button ] != isPressed )
         {
            m_ButtonStates[ button ] = isPressed;
            ButtonPressed?.Invoke( this, new ButtonPressedEventArgs( button, isPressed ) );
         }
      }
    }
}
