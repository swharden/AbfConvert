using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace AbfConvert.Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            string abfPath = System.IO.Path.GetFullPath("../../../../../data/2020_06_16_0000.abf");
            string fname = System.IO.Path.GetFileNameWithoutExtension(abfPath);
            using (var abf = new ABF(abfPath))
            {
                Console.WriteLine(abf);

                float[][] sweepValues = new float[abf.SweepCount][];
                for (int i = 0; i < abf.SweepCount; i++)
                {
                    sweepValues[i] = abf.GetSweep(i);
                    Console.WriteLine($"Sweep {i + 1} has {sweepValues[i].Length:N0} points");
                }

                Export.CSV(sweepValues, abfPath.Replace(".abf", ".csv"));
            }
        }
    }
}
