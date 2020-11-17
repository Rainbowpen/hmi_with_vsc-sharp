using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;  // write data to file.
using System.IO.Ports;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private SerialPort serialport = new SerialPort("COM10", 115200, Parity.None, 8, StopBits.One);//NEW一@個OSerialPort並LA連s接Ó£g埠Xe，A鮑j率v，A同P位i檢E查d，A資Me料A位i元¡M，A停X¡Ó止i位i元¡M
        char[] inbyte = new char[200];//建O立Ds一@個O陣X
        string[] sentc = {"A","B","C"};//要n傳C送Xe的o值E
        int flag = 0;
        int i = 0,
            num_get = 0,
            num_chart = 0,
            chart_xmax = 50;

        double temp_data,
               hum_data;

        public Form1()
        {
            InitializeComponent();
            serialport.ReadBufferSize = 1000;
            serialport.Open();
            timer1.Enabled = true;
            button1.Text = "Start";
            
            //button2.Text = "B2";
            label1.Text = "Temp";
            label2.Text = "";
            label3.Text = "Hum";
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            chart1.ChartAreas[0].AxisX.Maximum = chart_xmax;
            chart1.ChartAreas[0].AxisX.Minimum = num_chart;
            chart1.ChartAreas[0].AxisY.Maximum = 105;
            chart1.ChartAreas[0].AxisY.Minimum = -5;
            chart1.ChartAreas[0].AxisX.Interval = 10;
            chart1.ChartAreas[0].AxisY.Interval = 5;
            //comboBox1.SelectedIndex = 2;
            //chart1.ChartAreas[0].AxisX.fon

            chart1.Series[0].Name = "Temperature";
            chart1.Series[1].Name = "Humidity";
            serialport.Write("D");
            //label2.Text = "D";
            serialport.DiscardOutBuffer();

           // pictureBox1.Image = Resource1.hot;

            File.Create(@"D:\test.csv").Close(); // Create a file and close it.
            using (StreamWriter sw = File.AppendText(@"D:\test.csv"))
            {
                sw.WriteLine(hum_data.ToString() +  "," + temp_data.ToString());
                sw.Close();
            }
            
        }

        void serialport_datareceived(object sender, SerialDataReceivedEventArgs e)
        {
            
            if (serialport.BytesToRead >= 5)
            {
                flag = serialport.BytesToRead;
                serialport.Read(inbyte, 0, serialport.BytesToRead);
                serialport.DiscardInBuffer();
            }
           
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialport.Write(sentc[0]);
            serialport.DiscardOutBuffer();
            //if (get_status)
            //{
            serialport.Write("S");
            serialport.DiscardOutBuffer();

           
            //button1.Text = "Start";
            //label2.Text = "S";
                //get_status = false;
            //}
            /*else
            {
                serialport.Write("D");
                label2.Text = "D";
                serialport.DiscardOutBuffer();
                pictureBox1.Image = null;
                button1.Text = "Stop";
                get_status = true;
            }*/

        }
        /*
        private void button2_Click(object sender, EventArgs e)
        {
            serialport.Write(sentc[1]);
            //serialport.Write("D");
            serialport.DiscardOutBuffer();

        }
         * */

        private void timer1_Tick(object sender, EventArgs e)
        {
             serialport.DataReceived += new SerialDataReceivedEventHandler(serialport_datareceived);
            /* one word */
            /*if (flag >= 1)
            {
                label1.Text = Convert.ToString(inbyte[0]);
                //label3.Text = label3.Text + inbyte[0];
                if (Convert.ToString(inbyte[0]) == "N") label3.Text = "LED: On";
                else if (Convert.ToString(inbyte[0]) == "F") label3.Text = "LED: Off";
                if ((inbyte[0] > 0x30)&&(inbyte[0] <= 0x39))
                    label2.Text = Convert.ToString(inbyte[0]);
                
                if (inbyte[0] == 0x43)
                    label3.Text = "";

                flag = 0;
            }*/
            /* more word */

            if (flag >= 5)
            {
                //label1.Text = "";                
                string news0 = new String(inbyte);
                //label1.Text = news0;
                //label3.Text = label3.Text + news0;
                if ((inbyte[0] == 'T') && (inbyte[4] == 'E') || (inbyte[0] == 'H') && (inbyte[4] == 'E'))
                {
                    //label2.Text = "";
                    //label2.Text = news0;
                    num_get = 0;
                    for (i = 1; i <= 3; i++) {
                        if ((inbyte[i] >= 0x30) && (inbyte[i] <= 0x39))
                        {
                            num_get = num_get * 10 + (inbyte[i] - 0x30);
                        }
                        else {
                            break;
                        }
                    }
                    if (i == 4) { 
                        //label2.Text = (num_get * 2).ToString();

                        if (num_chart >= chart_xmax)
                        {
                            chart1.ChartAreas[0].AxisX.Maximum = num_chart;
                            chart1.ChartAreas[0].AxisX.Minimum = num_chart - chart_xmax;
                        }

                        if (inbyte[0] == 'T'){
                            chart1.Series[0].Points.AddY(num_get * 0.1);
                            num_chart++;
                            temp_data = num_get * 0.1;
                            label1.Text = "Temperature: " + (num_get * 0.1).ToString() + "°C";
                            if (num_get * 0.1 > 28)
                            {
                                pictureBox1.Image = Resource1.person_feel_hot;
                            }
                            else if ((num_get * 0.1 <= 28) && (num_get * 0.1 >= 22))
                            {
                                pictureBox1.Image = Resource1.person_feel_comfortable;
                            }
                            else if (num_get * 0.1 < 22)
                            {
                                pictureBox1.Image = Resource1.person_feel_cold;
                            }
                        }
                        else if (inbyte[0] == 'H'){
                            chart1.Series[1].Points.AddY(num_get * 0.1);
                            hum_data = num_get * 0.1;
                            label3.Text = "Humidity: " + (num_get * 0.1).ToString() + "%";

                            if (num_get * 0.1 > 70)
                            {
                                pictureBox2.Image = Resource1.raining_day;
                            }
                            else if ((num_get * 0.1 <= 70) && (num_get * 0.1 >= 40))
                            {
                                pictureBox2.Image = Resource1.tree;
                            }
                            else if (num_get * 0.1 < 40)
                            {
                                pictureBox2.Image = Resource1.cactus;
                            }
                        }
                        
                    }

                }
                

                //if (inbyte[0] == 0x43)
                    //label3.Text = "";

                for (i = 0; i < 200;i++ )
                {
                    inbyte[i] = Convert.ToChar(0x00);
                }
                    flag = 0;
            }
            /*
            char[] cin2;
            if (textBox1.TextLength > 0)
            {
                cin2 = textBox1.Text.ToCharArray();
                if ((cin2[0] >= '0') && (cin2[0] <= '9'))
                    label2.Text = (2 * (Convert.ToByte(cin2[0]) - 0x30)).ToString();
                else
                    label2.Text = "Input Error";
            }
            */

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click_1(object sender, EventArgs e)
        {

        }
        /*
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                serialport.Write("S");
                serialport.DiscardOutBuffer();

                pictureBox1.Image = Resource1.hot;
            }
            else {
                serialport.Write("D");
                serialport.DiscardOutBuffer();
                pictureBox1.Image = null;
            }
        }
         */
        /*
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) {
                chart1.Series[0].Enabled = true;
                chart1.Series[1].Enabled = false;
            }else if (comboBox1.SelectedIndex == 1)
            {

                chart1.Series[0].Enabled = false;
                chart1.Series[1].Enabled = true;

            }else if (comboBox1.SelectedIndex == 2)
            {

                chart1.Series[0].Enabled = true;
                chart1.Series[1].Enabled = true;

            }





        }
        */
    }
}
