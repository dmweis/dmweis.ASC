using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace dmweis.ASC.Connector.HardwareConnection
{
   class ArmConnector : IArmConnector, IDisposable
   {
      private readonly Task m_SerialWriterTask;
      private readonly BufferBlock<byte[]> m_CommandBuffer;
      private readonly string m_PortAddress;
      private readonly CancellationTokenSource m_CancellationTokenSource;

      public ArmConnector(SerialPortAddress portAddress) : this(portAddress.Name)
      {
      }

      public ArmConnector( string portAddress )
      {
         m_PortAddress = portAddress;
         m_CancellationTokenSource = new CancellationTokenSource();
         m_CommandBuffer = new BufferBlock<byte[]>();
         m_SerialWriterTask = Task.Factory.StartNew( ServoWriterLoopAsync, TaskCreationOptions.LongRunning );
      }

      public async Task SetMagnetAsync( bool turnOn )
      {
         byte[] byteArray = {(byte) ArmCommands.MagnetControll, (byte) (turnOn ? 1 : 0)};
         await m_CommandBuffer.SendAsync(byteArray);
      }

      public async Task MoveAllServosAsync( int baseServo, int shoulderServo, int elbowServo )
      {
         byte[] byteArray = new byte[ 7 ];
         byteArray[ 0 ] = (byte) ArmCommands.AllServoSet; // command number
         int pwm = baseServo;
         byteArray[ 1 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 2 ] = (byte) (pwm & 0xFF);
         pwm = shoulderServo;
         byteArray[ 3 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 4 ] = (byte) (pwm & 0xFF);
         pwm = elbowServo;
         byteArray[ 5 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 6 ] = (byte) (pwm & 0xFF);
         await m_CommandBuffer.SendAsync( byteArray );
      }

      public async Task MoveOneServoAsync( byte servoIndex, int servoPwm )
      {
         byte[] byteArray = new byte[ 4 ];
         byteArray[ 0 ] = (byte) ArmCommands.SingleServoSet; // command number
         byteArray[ 1 ] = (byte) servoIndex;
         byteArray[ 2 ] = (byte) ((servoPwm >> 8) & 0xFF);
         byteArray[ 3 ] = (byte) (servoPwm & 0xFF);
         await m_CommandBuffer.SendAsync(byteArray);
      }

      private async void ServoWriterLoopAsync()
      {
         CancellationToken cancellationToken = m_CancellationTokenSource.Token;
         using( SerialPort arduino = new SerialPort( m_PortAddress ) )
         {
            arduino.DtrEnable = false;
            arduino.Open();
            try
            {
               while (!cancellationToken.IsCancellationRequested)
               {
                  byte[] nextCommand = await m_CommandBuffer.ReceiveAsync(cancellationToken);
                  await arduino.WriteBytesAsync(nextCommand, cancellationToken);
               }
            }
            catch (TaskCanceledException)
            {
            }
         }
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            m_CancellationTokenSource?.Cancel();
            m_SerialWriterTask?.Dispose();
            m_CancellationTokenSource?.Dispose();
         }
      }
   }
}
