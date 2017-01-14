using System;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector.HardwareConnection
{
   static class SerialPortExtensions
   {
      public static Task WriteBytesAsync( this SerialPort @this, byte[] array, CancellationToken cancellationToken )
      {
         return @this.BaseStream.WriteAsync( array, 0, array.Length, cancellationToken );
      }

      public static Task WriteBytesAsync( this SerialPort @this, byte[] array )
      {
         return @this.WriteBytesAsync( array, CancellationToken.None );
      }

      public static async Task<byte[]> ReadAsync( this SerialPort @this, int maxSize, CancellationToken cancellationToken )
      {
         byte[] buffer = new byte[ maxSize ];
         int finalCount = 0;
         finalCount = await @this.BaseStream.ReadAsync( buffer, 0, maxSize, cancellationToken );
         byte[] tmpBuffer = new byte[ finalCount ];
         Array.Copy( buffer, tmpBuffer, finalCount );
         buffer = tmpBuffer;
         return buffer;
      }
   }
}
