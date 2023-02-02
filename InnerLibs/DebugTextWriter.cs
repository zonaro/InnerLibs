using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace InnerLibs
{
    public class DebugTextWriter : StreamWriter
    {
        #region Public Constructors

        public DebugTextWriter() : base(new DebugOutStream(), Encoding.Unicode, 1024)
        {
            AutoFlush = true;
        }

        #endregion Public Constructors

      
    }
      public sealed class DebugOutStream : Stream
        {
            #region Private Properties

            private static InvalidOperationException Bad_op => new InvalidOperationException("Operation not supported");

            #endregion Private Properties

            #region Public Properties

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw Bad_op;

            public override long Position
            {
                get => throw Bad_op;

                set => throw Bad_op;
            }

            #endregion Public Properties

            #region Public Methods

            public override void Flush() => Debug.Flush();

            public override int Read(byte[] buffer, int offset, int count) => throw Bad_op;

            public override long Seek(long offset, SeekOrigin origin) => throw Bad_op;

            public override void SetLength(long value) => throw Bad_op;

            public override void Write(byte[] buffer, int offset, int count) => Debug.Write(Encoding.Unicode.GetString(buffer, offset, count));

            #endregion Public Methods
        }

}