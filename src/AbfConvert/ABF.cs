using System;

namespace AbfConvert
{
    public class ABF : IDisposable
    {
        public readonly string Path;
        public readonly uint SweepCount = 0;
        public readonly uint SweepPointCount = 0;
        private readonly float[] sweepBuffer;

        // header details
        public readonly int SampleRate;
        public readonly double[] SweepStartTimes;

        private ABFFIO.ABFFileHeader header;
        private Int32 errorCode;
        private Int32 fileHandle;

        public ABF(string path)
        {
            Path = System.IO.Path.GetFullPath(path);
            if (!System.IO.File.Exists(path))
                throw new ArgumentException($"file does not exist: {path}");

            uint loadFlags = 0;
            SweepCount = 0;
            SweepPointCount = 0;
            header = new ABFFIO.ABFFileHeader();
            ABFFIO.ABF_ReadOpen(Path, ref fileHandle, loadFlags, ref header, ref SweepPointCount, ref SweepCount, ref errorCode);
            if (errorCode != 0)
                throw new ArgumentException($"ABFFIO failed to load ABF and read its header");
            sweepBuffer = new float[SweepPointCount];

            // store useful ABF information at the class level
            SampleRate = (int)(1e6 / header.fADCSequenceInterval / header.nADCNumChannels);

            SweepStartTimes = new double[SweepCount];
            if (header.fSynchTimeUnit > 0)
            {
                // fixed-length sweeps
                int sweepPointCount = header.lActualAcqLength / header.lActualEpisodes / header.nADCNumChannels;
                for (int i = 0; i < SweepCount; i++)
                    SweepStartTimes[i] = (double)(sweepPointCount * i) / SampleRate;
            }
            else
            {
                // variable length sweeps
                for (int i = 0; i < SweepCount; i++)
                {
                    int sweepNumber = i + 1;
                    Int32 syncCount = 0;
                    ABFFIO.ABF_SynchCountFromEpisode(fileHandle, ref header, sweepNumber, ref syncCount, ref errorCode);
                    if (errorCode != 0)
                        throw new ArgumentException($"ABFFIO failed to determine sweep time");
                    SweepStartTimes[i] = (double)syncCount / SampleRate;
                }
            }
        }

        public override string ToString()
        {
            return $"ABF file with {SweepCount} sweeps at {SampleRate} Hz";
        }

        public void Dispose()
        {
            ABFFIO.ABF_Close(fileHandle, ref errorCode);
            if (errorCode != 0)
                throw new ArgumentException($"ABFFIO failed to close ABF");
        }

        public float[] GetSweep(int sweepIndex = 1, int channelNumber = 0)
        {
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
