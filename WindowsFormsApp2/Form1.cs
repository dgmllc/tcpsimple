using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;           //for TcpClient
using System.Net;                   //for IPEndPoint
using System.Threading;             //for thread creation
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {

        //required globals for TCP communication
        private static TcpClient tcpClient = new TcpClient();
        private static NetworkStream stream;
        private static bool started = false;
        private static byte[] msg_rcv = new byte[1024];

        //delegate for talking between threads
        public delegate void textBox3_update_d(string passed_string);

        //Initialize the UI (look in Main.cs)
        public Form1()
        {
            InitializeComponent();
        }

        //connect to TCP Host
        private void connect(IPEndPoint pointer) {
            tcpClient.Connect(pointer);
            stream = tcpClient.GetStream();
            Thread t = new Thread(new ThreadStart(read));
            started = true;
            t.Start();
            textBox6.Text = "Started Thread";
        }

        //send a message to TCP host (requires connection to be already established)
        private void send(string outbound) {
            byte[] bytes = Encoding.ASCII.GetBytes(outbound);
            int size = bytes.Length;
            stream.Write(bytes,0,size);
        }

        //read a message from TCP host
        //this is started by connect() as a separate continuously running thread
        private void read() {
            //loop continously forever
            while (started) {
                //check receive buffer for a message
                if (tcpClient.ReceiveBufferSize > 0) {
                    //allocate buffer size
                    msg_rcv = new byte[tcpClient.ReceiveBufferSize];
                    //use stream objecto read in the bytes[]
                    stream.Read(msg_rcv, 0, tcpClient.ReceiveBufferSize);
                    //conver to string for printing
                    string msg = Encoding.ASCII.GetString(msg_rcv);
                    //delegate pointer to a thread which already exists
                    //this is required because this thread doesn't know where textBox3.Text lives.
                    textBox3.Invoke(new textBox3_update_d(this.textBox3_update), new object[] { msg });
                }
                //just sleep arbitrarily to prevent overrun?
                Thread.Sleep(1000);
            }
        }

        //used as a buffer between textBox3.text and read() for printing the received TCP message
        private void textBox3_update(string passed_string) {
            textBox3.Text = passed_string;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //ip address input, do nothing here
        }

        //Grab UI values and attempt to connect to the server IP and Port
        private void button1_Click(object sender, EventArgs e)
        {
            textBox6.Text = "IP Setup";

            //attemp to connect to controller
            string ip_add_t = textBox1.Text;
            string ip_port_t = textBox2.Text;
            int ip_port = Int32.Parse(ip_port_t);
            string ip_connect = ip_add_t + ":" + ip_port;
            textBox5.Text = ip_connect;

            //Uses a remote endpoint to establish a socket connection.
            IPAddress ipAddress = IPAddress.Parse(ip_add_t);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, ip_port);

            //doesn't need to be in a separate method, but I did it anyway
            connect(ipEndPoint);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            //do nothing
        }

        private void label4_Click(object sender, EventArgs e)
        {
            //do nothing
        }

        private void label6_Click(object sender, EventArgs e)
        {
            //do nothing
        }

        private void ip_send_Click(object sender, EventArgs e)
        {
            //attempt to send a message
            textBox6.Text = "Message Sent";
            string ip_msg = textBox4.Text;
            //doesn't need to be in another method, but I did it anyway
            send(ip_msg);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            //status update on connection, do nothing here
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //port address input, do nothing here
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //do nothing here
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //response input from controller, do nothing here
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            //where debug information gets printed, do nothing here
        }
    }
}
