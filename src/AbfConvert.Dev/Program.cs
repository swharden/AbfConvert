using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace AbfConvert.Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            ExportAll("../../../../../data/2020_06_16_0000.abf");
            ExportAll("../../../../../data/17o05028_ic_steps.abf");
        }

        static void ExportAll(string abfPath)
        {
            abfPath = System.IO.Path.GetFullPath(abfPath);
            Console.WriteLine($"Reading {abfPath}...");

            using var abf = new ABF(abfPath);
            Console.WriteLine(abf);

            float[][] sweepValues = new float[abf.SweepCount][];
            for (int i = 0; i < abf.SweepCount; i++)
            {
                sweepValues[i] = abf.GetSweep(i);
                Console.WriteLine($"Sweep {i + 1} has {sweepValues[i].Length:N0} points");
            }

            Console.WriteLine($"Exporting...");
            Export.CSV(sweepValues, abfPath.Replace(".abf", ".csv"), abf.SampleRate, abf.SweepStartTimes);
            Export.TSV(sweepValues, abfPath.Replace(".abf", ".tsv"), abf.SampleRate, abf.SweepStartTimes);
            Export.ATF(sweepValues, abfPath.Replace(".abf", ".atf"), abf.SampleRate, abf.SweepStartTimes);
        }
    }
}
