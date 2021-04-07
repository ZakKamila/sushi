using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace клиент
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static private string Exchange(string address, int port, string outMessage)
        {
            try
            {
                // Инициализация
                TcpClient client = new TcpClient(address, port);
                Byte[] data = Encoding.UTF8.GetBytes(outMessage);
                NetworkStream stream = client.GetStream();
                try
                {
                    // Отправка сообщения
                    stream.Write(data, 0, data.Length);
                    // Получение ответа
                    Byte[] readingData = new Byte[256];
                    String responseData = String.Empty;
                    StringBuilder completeMessage = new StringBuilder();
                    int numberOfBytesRead = 0;
                    do
                    {
                        numberOfBytesRead = stream.Read(readingData, 0, readingData.Length);
                        completeMessage.AppendFormat("{0}", Encoding.UTF8.GetString(readingData, 0, numberOfBytesRead));
                    }
                    while (stream.DataAvailable);
                    responseData = completeMessage.ToString();
                    return responseData;
                }
                finally
                {
                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception)
            {
                return ("Ожидание сервера...");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str;

            if (Exchange("127.0.0.1", 8888, $"check@we") != "no")
            {
                str = Exchange("127.0.0.1", 8888, $"view@we");
                string[] comand = str.Split(new char[] { '@' });
                str = "";
                for (int i = 0; i < comand.Length; i++)
                {
                    str += $"{comand[i]}\n";
                }
                MessageBox.Show(str);
            }
            else
                MessageBox.Show("в корзине пусто");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Order form = new Order();
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Menu form = new Menu();
            form.ShowDialog();
        }
    }
}
