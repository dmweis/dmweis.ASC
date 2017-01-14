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
      //private static readonly byte[] c_IdData = { 42, 96, 42, 96, 42, 96, 42, 96, 42 };
      private static readonly byte[] c_ServoDateIdentifier = {0, 1, 2};

      private readonly BufferBlock<byte[]> m_CommandBuffer = new BufferBlock<byte[]>();
      private readonly BufferBlock<byte[]> m_InputBuffer = new BufferBlock<byte[]>();
      private readonly Task m_SerialWriterTask;
      private readonly Task m_SerialReaderTask;
      private readonly Task m_PropagatorTask;
      private readonly CancellationTokenSource m_CancellationTokenSource;
      private readonly SerialPort m_Port;
      private readonly string m_PortAddress;

      public ServoPositions LastSetServoPositions { get; private set; }

      public event EventHandler<ArmDataUpdateEvent> NewServoPosition; 

      public ArmConnector(SerialPortAddress portAddress) : this(portAddress.Name)
      {
      }

      public ArmConnector( string portAddress )
      {
         m_PortAddress = portAddress;
         m_CancellationTokenSource = new CancellationTokenSource();
         m_Port = new SerialPort( m_PortAddress );
         m_Port.DtrEnable = false;
         m_Port.Open();
         m_Port.BaseStream.ReadTimeout = SerialPort.InfiniteTimeout;
         m_SerialWriterTask = Task.Factory.StartNew( ArduinoWriterLoopAsync, TaskCreationOptions.LongRunning );
         m_SerialReaderTask = Task.Factory.StartNew( ArduinoReadLoopAsync, TaskCreationOptions.LongRunning );
         m_PropagatorTask = Task.Factory.StartNew( PropagatorLoopAsync, TaskCreationOptions.LongRunning );
      }

      public async Task SetMagnetAsync( bool turnOn )
      {
         byte[] byteArray = {(byte) ArmCommands.MagnetControll, (byte) (turnOn ? 1 : 0)};
         await m_CommandBuffer.SendAsync(byteArray);
      }

      public async Task MoveAllServosAsync( ServoPositions position )
      {
         LastSetServoPositions = position;
         byte[] byteArray = new byte[ 7 ];
         byteArray[ 0 ] = (byte) ArmCommands.AllServoSet; // command number
         int pwm = position.Base.RoundToInt();
         byteArray[ 1 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 2 ] = (byte) (pwm & 0xFF);
         pwm = position.Shoulder.RoundToInt();
         byteArray[ 3 ] = (byte) ((pwm >> 8) & 0xFF);
         byteArray[ 4 ] = (byte) (pwm & 0xFF);
         pwm = position.Elbow.RoundToInt();
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

      public async Task SetServoSpeed(byte speed)
      {
         byte[] byteArray = new byte[2];
         byteArray[0] = (byte) ArmCommands.ServoSpeed; // command number
         byteArray[1] = speed;
         await m_CommandBuffer.SendAsync(byteArray);
      }

      private async void ArduinoWriterLoopAsync()
      {
         CancellationToken cancellationToken = m_CancellationTokenSource.Token;
         try
         {
            while (!cancellationToken.IsCancellationRequested)
            {
               byte[] nextCommand = await m_CommandBuffer.ReceiveAsync(cancellationToken);
               await m_Port.WriteBytesAsync(nextCommand, cancellationToken);
            }
         }
         catch (TaskCanceledException)
         {
         }
      }

      private async void ArduinoReadLoopAsync()
      {
         CancellationToken cancellationToken = m_CancellationTokenSource.Token;
         try
         {
            while ( !cancellationToken.IsCancellationRequested )
            {
               byte[] incomingData = await m_Port.ReadAsync(10, cancellationToken);
               await m_InputBuffer.SendAsync(incomingData, cancellationToken);
            }
         }
         catch (TaskCanceledException)
         {
         }
      }

      private async void PropagatorLoopAsync()
      {
         CancellationToken cancellationToken = m_CancellationTokenSource.Token;
         try
         {
            int counter = 0;
            byte[] data = new byte[6];
            while ( !cancellationToken.IsCancellationRequested )
            {
               byte[] incomingData = await m_InputBuffer.ReceiveAsync(cancellationToken);
               foreach (var incomingByte in incomingData )
               {
                  if (counter > 2)
                  {
                     data[counter - c_ServoDateIdentifier.Length] = incomingByte;
                     if ( counter - c_ServoDateIdentifier.Length == data.Length - 1)
                     {
                        int baseAngle = (data[0] << 8) | data[1];
                        int shoulderAngle = (data[2] << 8) | data[3];
                        int elbowAngle = (data[4] << 8) | data[5];
                        var currentServoPosition = new ServoPositions(baseAngle, shoulderAngle, elbowAngle);
                        NewServoPosition?.Invoke(this, new ArmDataUpdateEvent(currentServoPosition, LastSetServoPositions));
                        counter = 0;
                     }
                     else
                     {
                        counter++;
                     }
                  }
                  else if (incomingByte == c_ServoDateIdentifier[counter])
                  {
                     counter++;
                  }
               }
            }
         }
         catch (TaskCanceledException)
         {
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
