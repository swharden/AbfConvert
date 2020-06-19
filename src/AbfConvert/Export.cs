using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AbfConvert
{
    public static class Export
    {
        private static string[,] MakeText(float[][] jagged)
        {
            int columns = jagged.Length;
            int rows = jagged.Select(x => x.Length).Max();
            string[,] txt = new string[columns + 1, rows];
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                for (int rowIndex = 0; rowIndex < jagged[columnIndex].Length; rowIndex++)
                    txt[columnIndex, rowIndex] = $"{jagged[columnIndex][rowIndex]}";
            return txt;
        }

        public static void TSV(float[][] data, string path, int sampleRate, double[] sweepStartTimes)
        {
            SV(data, path, sampleRate, sweepStartTimes, '\t');
        }

        public static void CSV(float[][] data, string path, int sampleRate, double[] sweepStartTimes)
        {
            SV(data, path, sampleRate, sweepStartTimes, ',');
        }

        private static void SV(float[][] data, string path, int sampleRate, double[] sweepStartTimes, char separator)
        {
            string[,] valuesText = MakeText(data);

            StringBuilder sb = new StringBuilder();
            sb.Append($"Sweep Start (s){separator} ");
            for (int i = 0; i < data.Length; i++)
                sb.Append($"{(float)sweepStartTimes[i]}{separator} ");
            sb.AppendLine();

            sb.Append($"Time (s){separator} ");
            for (int i = 1; i < valuesText.GetLength(0); i++)
                sb.Append($"Sweep #{i}{separator} ");
            sb.AppendLine();

            for (int rowIndex = 0; rowIndex < valuesText.GetLength(1); rowIndex++)
            {
                double rowTimeSec = (double)rowIndex / sampleRate;
                sb.Append($"{rowTimeSec}{separator} ");
                for (int columnIndex = 0; columnIndex < data.Length; columnIndex++)
                    sb.Append($"{valuesText[columnIndex + 1, rowIndex]}{separator} ");
                sb.AppendLine();
            }

            System.IO.File.WriteAllText(path, sb.ToString());
        }

        public static void ATF(float[][] data, string path, int sampleRate, double[] sweepStartTimes)
        {
            string[,] valuesText = MakeText(data);

            // first line indicates file format
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ATF\t1.0");

            // second line indicates header length and data width
            int headerLinesAfterThis = 2;
            int dataColumnCount = valuesText.GetLength(0) + 1;
            sb.AppendLine($"{headerLinesAfterThis}\t{dataColumnCount}");

            // extra line 1 = start time for each sweep
            sb.Append("\"SweepStartTimesMS=");
            for (int i = 0; i < data.Length; i++)
                sb.Append($"{(float)sweepStartTimes[i] * 1000},");
            sb.AppendLine("\"");

            // last line of header = column labels
            sb.Append($"\"Time (ms)\"\t");
            for (int i = 1; i < valuesText.GetLength(0); i++)
                sb.Append($"\"Trace #{i}\"\t");
            sb.AppendLine();

            for (int rowIndex = 0; rowIndex < valuesText.GetLength(1); rowIndex++)
            {
                double rowTimeSec = (double)rowIndex / sampleRate;
                sb.Append($"{rowTimeSec * 1000}\t");
                for (int columnIndex = 0; columnIndex < data.Length; columnIndex++)
                    sb.Append($"{valuesText[columnIndex + 1, rowIndex]}\t");
                sb.AppendLine();
            }

            System.IO.File.WriteAllText(path, sb.ToString());
        }
    }
}
