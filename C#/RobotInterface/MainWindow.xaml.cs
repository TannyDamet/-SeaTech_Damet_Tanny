using ExtendedSerialPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotInterface
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ReliableSerialPort serialPort1;

        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM15", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            serialPort1.OnDataReceivedEvent += SerialPort1_OnDataReceivedEvent;
            serialPort1.Open();

        }

        private void SerialPort1_OnDataReceivedEvent(object sender, DataReceivedArgs e)
        {
            //throw new NotImplementedException();
        }

        bool toggle = false;


        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {

            if (toggle == true)

                buttonEnvoyer.Background = Brushes.RoyalBlue;
            else
                buttonEnvoyer.Background = Brushes.Gold;

            toggle = !toggle;

            SendMessage();
        }
        void SendMessage()
        {
            //string messageEmis = textboxEmission.Text;
            //textboxEmission.Text = "";
            //textboxRéception.Text += "\nRéçu : " + messageEmis;
            serialPort1.WriteLine(textboxEmission.Text);
        }

        private void TextBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

    }

    

    

}
    