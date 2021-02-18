using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Common;

namespace Wave
{
    public class Wave
    {
        public List<Chunk> ChunkList { get; set; } = new List<Chunk>();
        public Chunk FormatChunk { get; private set; } = new Chunk { Name = "", Size = 0, Offset = 0 };
        public Chunk DataChunk { get; private set; } = new Chunk { Name = "", Size = 0, Offset = 0 };        
        public short WaveTypeId { get; private set; } = 0;
        public short NumChannels { get; private set; } = 0;
        public int SampleRate { get; private set; } = 0;
        public int ByteRate { get; private set; } = 0;
        public short BlockAlign { get; private set; } = 0;
        public short BitsPerSample { get; private set; } = 0;
        public double AudioLength { get; private set; } = 0.0;
        public byte[] DataBytes { get; private set; } = null;
        private bool IsDeep = false;
        public void ShallowParse(string wavePath)
        {
            Parse(wavePath, false);
        }

        public void DeepParse(string wavePath)
        {
            Parse(wavePath, true);
        }

        private void Parse(string wavePath, bool isDeep = false)
        {
            using(Stream st=new FileStream(wavePath, FileMode.Open, FileAccess.Read))
            {
                Parse(st, isDeep);
            }
        }

        private void Parse(Stream st, bool isDeep = false)
        {
            IsDeep = isDeep;
            ParseHeader(st);
            ParseRecursively(st);
            CheckFormat(st);
            CheckData(st);
        }

        private void ParseHeader(Stream st)
        {
            Sanity.Requires(st.Length >= 44, $"Audio size is too small. Expected at least 44, which is actually {st.Length} ");
            Sanity.Requires(st.Length <= int.MaxValue, $"Audio size is too large, more than 2GB.");
            Sanity.Requires(st.ReadBytesToString(4, Encoding.ASCII) == "RIFF", $"File type error, not RIFF.");
            Sanity.Requires(st.ReadBytesToInt32() + 8 == st.Length, $"File length error, in RIFF chunk.");
            Sanity.Requires(st.ReadBytesToString(4, Encoding.ASCII) == "WAVE", $"File type error, not WAVE");            
        }

        private void ParseRecursively(Stream st)
        {
            if (st.Position == st.Length)
                return;
            Sanity.Requires(st.Position + 8 <= st.Length, $"Audio length too short, at 0x{st.Position:x}.");
            string chunkName = st.ReadBytesToString(4, Encoding.ASCII);
            int chunkSize = st.ReadBytesToInt32();
            Chunk chunk = new Chunk { Name = chunkName, Size = chunkSize, Offset = (int)st.Position };
            switch (chunkName)
            {
                case "fmt ":
                    Sanity.Requires(FormatChunk.Name != "", "Dupelicate format chunk.");
                    FormatChunk = chunk;
                    return;
                case "data":
                    Sanity.Requires(DataChunk.Name != "", "Duplicate data chunk.");
                    DataChunk = chunk;
                    return;
                default:
                    break;
            }
            Sanity.Requires(st.Position + chunkSize <= st.Length, $"Audio length too short, at {chunkName}, at 0x{st.Position:x}.");
            st.Seek(chunkSize, SeekOrigin.Current);
            ParseRecursively(st);
        }

        private void CheckFormat(Stream st)
        {
            Sanity.Requires(FormatChunk.Name != "", "No format chunk.");
            Sanity.Requires(FormatChunk.Size >= 16, $"Format chunk too small. Expected at least 16, which is actually {FormatChunk.Size}.");
            st.Seek(FormatChunk.Offset, SeekOrigin.Begin);
            WaveTypeId = st.ReadBytesToInt16();
            NumChannels = st.ReadBytesToInt16();
            SampleRate = st.ReadBytesToInt32();
            ByteRate = st.ReadBytesToInt32();
            BlockAlign = st.ReadBytesToInt16();
            BitsPerSample = st.ReadBytesToInt16();
            Sanity.Requires(ByteRate == SampleRate * BlockAlign, $"Wave format error: byte rate: {ByteRate}, sample rate: {SampleRate}, block align: {BlockAlign}.");
            Sanity.Requires(BitsPerSample * NumChannels == 8 * BlockAlign, $"Wave format error: bits per sample: {BitsPerSample}, channel: {NumChannels}, block align: {BlockAlign}.");
            AudioLength = (double)DataChunk.Size / ByteRate;
        }

        private void CheckData(Stream st)
        {
            Sanity.Requires(DataChunk.Name != "", "No data chunk.");
            if (IsDeep)
            {
                DataBytes = new byte[DataChunk.Size];
                st.Seek(DataChunk.Offset, 0);
                st.Read(DataBytes, 0, DataChunk.Size);
            }
        }
    }
    public struct Chunk
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public int Offset { get; set; }
    }
}