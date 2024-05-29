using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace FibonacciProgressBar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            int i = 20; // i-ty wyraz ci¹gu Fibonacciego

            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            worker.RunWorkerAsync(i);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = (int)e.Argument;
            int a = 0;
            int b = 1;

            for (int j = 0; j <= i; j++)
            {
                Thread.Sleep(100);
                int temp = a;
                a = b;
                b = temp + b;

                int progressPercentage = (int)((j / (float)i) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage, a);
            }

            e.Result = a;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            this.Text = $"Progress: {e.ProgressPercentage}% - Current Fibonacci Number: {e.UserState}";
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 100;
            MessageBox.Show($"The {e.Result} is the i-th Fibonacci number.");
        }
    }
}
