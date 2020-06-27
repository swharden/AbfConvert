using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbfConvert.GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tbOutFolder.Text = Path.GetFullPath("./convertedABFs");
            Version ver = typeof(ABF).Assembly.GetName().Version;
            lblStatus.Text = $"AbfConvert v{ver.Major}.{ver.Minor}";
            progress.Value = 0;
        }

        private void lbABFs_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnConvert.Enabled = lbABFs.Items.Count > 0;
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/swharden/AbfConvert");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            lbABFs.Items.Clear();
        }

        private void btnOutFolderSet_Click(object sender, EventArgs e)
        {
            var diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == DialogResult.OK)
                tbOutFolder.Text = diag.SelectedPath;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            Enabled = false;

            if (!Directory.Exists(tbOutFolder.Text))
                Directory.CreateDirectory(tbOutFolder.Text);

            progress.Maximum = lbABFs.Items.Count;
            for (int i = 0; i < lbABFs.Items.Count; i++)
            {
                string abfPath = lbABFs.Items[i].ToString();
                string abfID = Path.GetFileNameWithoutExtension(abfPath);
                lblStatus.Text = $"Converting {abfID}.abf...";
                progress.Value = i + 1;
                Application.DoEvents();

                var abf = new ABF(abfPath);
                float[][] sweepValues = new float[abf.SweepCount][];
                for (int j = 0; j < abf.SweepCount; j++)
                    sweepValues[j] = abf.GetSweep(j);

                if (rbCSV.Checked)
                {
                    string pathOut = Path.Combine(tbOutFolder.Text, abfID + ".csv");
                    Export.CSV(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                }
                else if (rbTSV.Checked)
                {
                    string pathOut = Path.Combine(tbOutFolder.Text, abfID + ".tsv");
                    Export.TSV(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                }
                else if (rbATF.Checked)
                {

                    string pathOut = Path.Combine(tbOutFolder.Text, abfID + ".atf");
                    Export.ATF(sweepValues, pathOut, abf.SampleRate, abf.SweepStartTimes);
                }
            }

            progress.Value = 0;
            lblStatus.Text = $"Finished converting {lbABFs.Items.Count} ABFs.";
            Enabled = true;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    string[] abfPaths = Directory.GetFiles(path, "*.abf");
                    lbABFs.Items.AddRange(abfPaths);
                }
                else if (File.Exists(path))
                {
                    if (path.EndsWith(".abf", StringComparison.OrdinalIgnoreCase))
                        lbABFs.Items.Add(path);
                }
            }
        }
    }
}
