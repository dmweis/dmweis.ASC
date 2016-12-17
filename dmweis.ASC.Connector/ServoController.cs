using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
    public class ServoController
    {
        private SerialPort m_Arduino;
        private int[] m_ServoValues;

        public ServoController( string comPort )
        {
            m_ServoValues = new int[ 16 ];
         m_Arduino = new SerialPort( comPort, 9600 )
         {
            DtrEnable = false
         };
         m_Arduino.Open();
            //m_Task = Task.Factory.StartNew( ServoUpdateLoop, TaskCreationOptions.LongRunning );
        }

        private void SetServoInternal( int numberOfServo, int pwm )
        {
            m_ServoValues[ numberOfServo ] = pwm;
        }

        public void SetServo( int numberOfServo, int pwm )
        {
            byte[] byteArray = new byte[3];
            byteArray[ 0 ] = (byte)numberOfServo;
            byteArray[ 1 ] = (byte)( ( pwm >> 8 ) & 0xFF );
            byteArray[ 2 ] = (byte)( pwm & 0xFF );
            m_Arduino.Write( byteArray, 0, 3 );
        }

        private async void ServoUpdateLoopAsync()
        {
            while( true )
            {
                for( int index = 0 ; index < m_ServoValues.Length ; index++ )
                {
                    SetServoInternal( index , m_ServoValues[ index ] );
                    await Task.Delay( 10 );
                }
                await Task.Delay( 50 );
            }
        }
    }
}
