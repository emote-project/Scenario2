using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Linq;
using System.Windows;
using System.Timers;
using System.Reflection;
using System.Threading;
using PipeClient;


namespace Qmodule
{

    public partial class Form1 : Form
    {
        string RxString;
        Client pipec;
       // System.IO.Ports.SerialPort serialPort1=new System.IO.Ports.SerialPort();
        public Form1()
        {
            this.pipec = new Client();
            pipec.MessageReceived +=
                new Client.MessageReceivedHandler(pipeClient_MessageReceived);
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
           if(pipec.Connected==true) pipec.Stop();
            Application.Exit();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen==true) serialPort1.Close();

            serialPort1.PortName = textBox2.Text;
            serialPort1.BaudRate = 115200;
            serialPort1.ReadTimeout = 10000;
            serialPort1.WriteTimeout = 10000;
            try
            {
                serialPort1.Open();
            }
            catch (Exception ex)
            { MessageBox.Show("Cannot open comm port. Try again 3 seconds after you pressed the Q Sensor button",ex.Message); }

            if (serialPort1.IsOpen)
            {
                buttonStart.Enabled = false;
                textBox1.ReadOnly = true;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                buttonStart.Enabled = true;
                button2.Enabled = false;
                textBox1.ReadOnly = true;
                textBox1.Clear();

            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
            if (pipec.Connected == true) pipec.Stop();
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if(pipec.Connected==true)
            {
                RxString = serialPort1.ReadLine();
                this.Invoke(new EventHandler(DisplayText));
            }
            else
            {
                serialPort1.DiscardInBuffer();
            }

        }
        private void DisplayText(object sender, EventArgs e)
        {
            textBox1.AppendText(DateTime.Now + ":" + DateTime.Now.Millisecond.ToString("000") + "," + RxString + System.Environment.NewLine);
            try
            {
                pipec.SendMessage("Temp:" + (RxString.Substring(RxString.Length - 11, 4)) + " EDA:" + (RxString.Substring(RxString.Length - 6)));
                // thalamus.qPublisher.PerceptionQtemp(float.Parse(RxString.Substring(RxString.Length-10,4))); 
                // thalamus.qPublisher.PerceptionQeda(float.Parse(RxString.Substring(RxString.Length-5)));
            }
            catch (Exception a) {  }
                serialPort1.DiscardInBuffer();

        }
        void pipeClient_MessageReceived(string message)
        {
            Invoke(new PipeClient.Client.MessageReceivedHandler(DisplayReceivedMessage),
            new object[] { message });
        }
        void DisplayReceivedMessage(string message)
        {
            //MessageBox.Show("meesage" + message);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!pipec.Connected)
            {
                pipec.PipeName = "\\\\.\\pipe\\serverpipe";
                pipec.Connect();
                if(pipec.Connected==true)
                this.button3.Enabled = false;

            }
            else
                MessageBox.Show("Already connected");
        }

    }
}

