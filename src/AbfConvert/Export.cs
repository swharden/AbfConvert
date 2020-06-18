﻿using System;
using System.Collections.Generic;
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
            string[,] txt = new string[columns, rows];
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
                for (int rowIndex = 0; rowIndex < jagged[columnIndex].Length; rowIndex++)
                    txt[columnIndex, rowIndex] = $"{jagged[columnIndex][rowIndex]}";
            return txt;
        }

        public static void CSV(float[][] data, string path)
        {
            string[,] valuesText = MakeText(data);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# CSV file generated by AbfConvert");
            for (int rowIndex = 0; rowIndex < valuesText.GetLength(1); rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < valuesText.GetLength(0); columnIndex++)
                {
                    sb.Append(valuesText[columnIndex, rowIndex] + ", ");
                }
                sb.AppendLine();
            }

            System.IO.File.WriteAllText(path, sb.ToString());
        }
    }
}