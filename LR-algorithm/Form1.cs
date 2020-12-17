using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace LR_algorithm
{
    public partial class Form1 : Form
    {
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера
        public Form1() {
            InitializeComponent();
        }

        private void btnRes_Click(object sender, EventArgs e) {
            tboxVec.Clear();
            tboxZn.Clear();
            labelMessage.Text = "";

            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text)
                && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox2.Text)
                && !string.IsNullOrEmpty(textBox3.Text) && !string.IsNullOrWhiteSpace(textBox3.Text)
                && !string.IsNullOrEmpty(textBox4.Text) && !string.IsNullOrWhiteSpace(textBox4.Text)
                && !string.IsNullOrEmpty(textBox5.Text) && !string.IsNullOrWhiteSpace(textBox5.Text)
                && !string.IsNullOrEmpty(textBox6.Text) && !string.IsNullOrWhiteSpace(textBox6.Text)
                && !string.IsNullOrEmpty(textBox7.Text) && !string.IsNullOrWhiteSpace(textBox7.Text)
                && !string.IsNullOrEmpty(textBox8.Text) && !string.IsNullOrWhiteSpace(textBox8.Text)
                && !string.IsNullOrEmpty(textBox9.Text) && !string.IsNullOrWhiteSpace(textBox9.Text)) {
                try {
                    IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ipPoint);
                    string sdata = textBox1.Text + ";" +
                        textBox2.Text + ";" +
                        textBox3.Text + ";" +
                        textBox4.Text + ";" +
                        textBox5.Text + ";" +
                        textBox6.Text + ";" +
                        textBox7.Text + ";" +
                        textBox8.Text + ";" +
                        textBox9.Text;
                    byte[] bdata = Encoding.UTF8.GetBytes(sdata);
                    socket.Send(bdata);

                    //получаем ответ

                    byte[] bAnswer = new byte[1024];
                    int bytesRec = socket.Receive(bAnswer);
                    string sAnswer = Encoding.UTF8.GetString(bAnswer, 0, bytesRec);

                    string[] answers = sAnswer.Split('_');
                    labelMessage.Text = answers[0];
                    tboxZn.Text += answers[1];
                    tboxVec.Text += answers[2];

                    // закрываем сокет

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            else
                labelMessage.Text = "Введите значения во все поля.";
        }
    }
}
