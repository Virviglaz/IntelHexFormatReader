using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntelHexFormatReader.Model;

namespace IntelHexFormatReader
{
    public class HexFileReader
    {
        private IEnumerable<string> hexRecordLines;
        private int memorySize;

        #region Constructors

        public HexFileReader(string fileName)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException(string.Format("File {0} does not exist!", fileName));
            Initialize(File.ReadLines(fileName));
        }

        public HexFileReader(IEnumerable<string> hexFileContents)
        {
            Initialize(hexFileContents);
        }

        #endregion

        private void Initialize(IEnumerable<string> lines)
        {
            var fileContents = lines as IList<string> ?? lines.ToList();
            if (!fileContents.Any()) throw new ArgumentException("Hex file contents can not be empty!");
            hexRecordLines = fileContents;
        }

        /// <summary>
        /// Parse the currently loaded HEX file contents.
        /// </summary>
        /// <returns>A MemoryBlock representation of the HEX file.</returns>
        public MemoryBlock Parse()
        {
            return ReadHexFile(hexRecordLines);
        }

        private static MemoryBlock ReadHexFile(IEnumerable<string> hexRecordLines)
        {
            var result = new MemoryBlock();

            uint baseAddress = 0;
            var encounteredEndOfFile = false;
            foreach (var hexRecordLine in hexRecordLines)
            {
                var hexRecord = HexFileLineParser.ParseLine(hexRecordLine);
                switch (hexRecord.RecordType)
                {
                    case RecordType.Data:
                        {
                            var nextAddress = hexRecord.Address + baseAddress;
                            for (uint i = 0; i < hexRecord.ByteCount; i++)
                            {
                                result.Add(new MemoryCell(nextAddress + i, hexRecord.Bytes[i]));
                            }
                            break;
                        }
                    case RecordType.EndOfFile:
                        {
                            hexRecord.Assert(rec => rec.Address == 0, "Address should equal zero in EOF.");
                            hexRecord.Assert(rec => rec.ByteCount == 0, "Byte count should be zero in EOF.");
                            hexRecord.Assert(rec => rec.Bytes.Length == 0, "Number of bytes should be zero for EOF.");
                            hexRecord.Assert(rec => rec.CheckSum == 0xff, "Checksum should be 0xff for EOF.");
                            encounteredEndOfFile = true;
                            break;
                        }
                    case RecordType.ExtendedSegmentAddress:
                        {
                            hexRecord.Assert(rec => rec.ByteCount == 2, "Byte count should be 2.");
                            baseAddress = (uint)(hexRecord.Bytes[0] << 8 | hexRecord.Bytes[1]) << 4;
                            break;
                        }
                    case RecordType.ExtendedLinearAddress:
                        {
                            hexRecord.Assert(rec => rec.ByteCount == 2, "Byte count should be 2.");
                            baseAddress = (uint)(hexRecord.Bytes[0] << 8 | hexRecord.Bytes[1]) << 16;
                            break;
                        }
                }
            }
            if (!encounteredEndOfFile) throw new IOException("No EndOfFile marker found!");
            return result;
        }
    }
}
