﻿using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Diagnostics;

namespace DtaSpy
{
    public class BizTalkFragmentBlockWriter: IBlockWriter
    {
        public Stream output { get; set; }
        private static readonly byte[] zeroBuf = new byte[] { 0, 0, 0 };

        public BizTalkFragmentBlockWriter(Stream output)
        {
            this.output = output;
        }

        public void WriteBlock(FragmentBlock block)
        {
            WriteBlock(block.Content, 0, block.Length, block.Compressed, block.UncompressedLength);
        }

        public void WriteBlock(byte[] buffer, int offset, int count, bool compressed, int uncompressedLength)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Offset cannot be a negative number");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Count cannot be a negative number");

            if ((buffer.Length - offset) < count)
                throw new ArgumentException("Offset and count would exceed buffer lenght");

            var bw = new BinaryWriter(this.output);

            bw.Write(compressed);

            // I don't know what these bytes represent
            bw.Write(zeroBuf, 0, 3);

            // Content length (uncompressed length) 16bit little endian
            bw.Write((ushort)uncompressedLength);

            // I don't know what these bytes represent
            bw.Write(zeroBuf, 0, 2);

            // 16bit little endian
            bw.Write((ushort)count);

            // I don't know what these bytes represent
            bw.Write(zeroBuf, 0, 2);

            if (count > 0)
                this.output.Write(buffer, offset, count);
        }

        public void Flush()
        {
            this.output.Flush();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this.output.Close();
        }

        public void Close()
        {
            this.Dispose(true);
        }
    }
}