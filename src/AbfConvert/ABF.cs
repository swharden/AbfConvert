using System;

namespace AbfConvert
{
    public class ABF : IDisposable
    {
        public readonly string Path;
        public readonly uint SweepCount = 0;
        public readonly uint SweepPointCount = 0;
        private readonly float[] sweepBuffer;

        private ABFFIO.ABFFileHeader header;

        public ABF(string path)
        {
            Path = System.IO.Path.GetFullPath(path);
            if (!System.IO.File.Exists(path))
                throw new ArgumentException($"file does not exist: {path}");

            Int32 errorCode = 0;
            Int32 fileHandle = 0;
            uint loadFlags = 0;
            SweepCount = 0;
            SweepPointCount = 0;
            header = new ABFFIO.ABFFileHeader();
            ABFFIO.ABF_ReadOpen(Path, ref fileHandle, loadFlags, ref header, ref SweepPointCount, ref SweepCount, ref errorCode);
            if (errorCode != 0)
                throw new ArgumentException($"ABFFIO failed to load ABF and read its header");
            sweepBuffer = new float[SweepPointCount];
        }

        public override string ToString()
        {
            return $"ABF file with {SweepCount} sweeps";
        }

        public void Dispose()
        {
            Int32 fileHandle = 0;
            Int32 errorCode = 0;
            ABFFIO.ABF_Close(fileHandle, ref errorCode);
            if (errorCode != 0)
                throw new ArgumentException($"ABFFIO failed to close ABF");
        }

        public float[] GetSweep(int sweepIndex = 1, int channelNumber = 0)
        {
            Int32 errorCode = 0;
            Int32 fileHandle = 0;
            int physicalChannel = header.nADCSamplingSeq[channelNumber];
            uint thisSweepPointCount = 0;
            ABFFIO.ABF_ReadChannel(fileHandle, ref header, physicalChannel, sweepIndex + 1, ref sweepBuffer[0], ref thisSweepPointCount, ref errorCode);
            if (errorCode != 0)
                throw new ArgumentException($"ABFFIO failed to read channel {channelNumber} sweep {sweepIndex}");
            
            float[] thisSweep = new float[thisSweepPointCount];
            Array.Copy(sweepBuffer, 0, thisSweep, 0, thisSweepPointCount);
            return thisSweep;
        }
    }
}
