using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dmweis.ASC.Connector
{
   static class SerialPortExtensions
   {
      public static Task WriteBytesAsync( this SerialPort @this, byte[] array, CancellationToken cancelationToken )
      {
         return @this.BaseStream.WriteAsync( array, 0, array.Length );
      }

      public static Task WriteBytesAsync( this SerialPort @this, byte[] array )
      {
         return @this.WriteBytesAsync( array, CancellationToken.None );
      }

      public static async Task<byte[]> ReadAsync( this SerialPort @this, int maxSize, int millisTimeout )
      {
         byte[] buffer = new byte[ maxSize ];
         using( CancellationTokenSource tokenSource = new CancellationTokenSource( millisTimeout ) )
         {
            int finalCount = 0;
            try
            {
               finalCount = await @this.BaseStream.ReadAsync( buffer, 0, maxSize, tokenSource.Token );
            }
            catch( OperationCanceledException ){}
            byte[] tmpBuffer = new byte[ finalCount ];
            Array.Copy( buffer, tmpBuffer, finalCount );
            buffer = tmpBuffer;
         }
         return buffer;
      }
   }
}
