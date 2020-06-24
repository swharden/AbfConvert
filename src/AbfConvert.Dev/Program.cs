using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace AbfConvert.Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            Version version = System.Reflection.Assembly.GetAssembly(typeof(ABF)).GetName().Version;
            Console.WriteLine($"AbfConvert {version.Major}.{version.Minor}\n");

            if (args.Length == 0)
            {
                Console.WriteLine($"ERROR: THIS PROGRAM MUST BE RUN FROM THE COMMAND PROMPT.");
                ShowHelp();
                Console.ReadLine();
                return;
            }

            if (args.Length != 3)
            {
                Console.WriteLine($"ERROR: invalid arguments");
                ShowHelp();
                return;
            }

            string pathIn = System.IO.Path.GetFullPath(args[0]);
            string pathOut = System.IO.Path.GetFullPath(args[1]);
            string format = args[2].Replace(".", "").ToUpper();
            string[] supportedFormats = { "CSV", "TSV", "ATF" };
            if (!supportedFormats.Contains(format))
            {
                Console.WriteLine($"ERROR: invalid format ({format})");
                ShowHelp();
                return;
            }

            if (System.IO.Directory.Exists(pathIn))
            {
                if (!System.IO.Directory.Exists(pathOut))
                    System.IO.Directory.CreateDirectory(pathOut);

                foreach (string abfPath in System.IO.Directory.GetFiles(pathIn, "*.abf"))
                    Convert(abfPath, $"{pathOut}/{System.IO.Path.GetFileNameWithoutExtension(abfPath)}.{format.ToLower()}", format);

                Console.WriteLine($"ABF conversions complete.");
            }
            else if (System.IO.File.Exists(pathIn))
            {
                Convert(pathIn, pathOut, format);
                Console.WriteLine($"ABF conversion complete.");
            }
            else
            {
                Console.WriteLine($"ERROR: invalid input path ({pathIn})");
                ShowHelp();
                return;
            }
        }

        static void Convert(string pathIn, string pathOut, string format)
        {
            Console.WriteLine($"Reading data from {System.IO.Path.GetFileName(pathIn)}...");

            using (var abf = new ABF(pathIn))
            {
                Console.WriteLine(abf);

                float[][] sweepValues = new float[abf.SweepCount][];
                for (int i = 0; i < abf.SweepCount; i++)
                    sweepValues[i] = abf.GetSweep(i);

                Console.WriteLine($"Creating {System.IO.Path.GetFileName(pathOut)}...");

                if (format == "CSV")
                    Export.CSV(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                else if (format == "TSV")
                    Export.TSV(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                else if (format == "ATF")
                    Export.ATF(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                else
                    throw new NotImplementedException($"Unsupported output format {format}");
            }
        }

        static void ShowHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine(" AbfConvert :: Convert ABF files to CSV, TSV, or ATF format");
            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine();
            sb.AppendLine("      Usage :: AbfConverter.exe [input] [output] [format]");
            sb.AppendLine();
            sb.AppendLine("      input :: path to an ABF file or a folder of ABF files");
            sb.AppendLine("     output :: path to the output file or folder");
            sb.AppendLine("     format :: CSV, TSV, or ATF");
            sb.AppendLine();
            sb.AppendLine("    Example :: AbfConvert.exe sample.abf sample.csv CSV");
            sb.AppendLine();
            sb.AppendLine("    Example :: AbfConvert.exe ./folderIn/ ./folderOut/ CSV");
            sb.AppendLine();
            Console.WriteLine(sb); ;
        }
    }
}
