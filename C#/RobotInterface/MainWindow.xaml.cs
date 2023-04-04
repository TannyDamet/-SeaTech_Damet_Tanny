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
using System.Windows.Threading;

namespace RobotInterface
{

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>




    public partial class MainWindow : Window
    {

        ReliableSerialPort serialPort1;
        DispatcherTimer timerAffichage = new DispatcherTimer();
        Robot robot = new Robot();
        
        bool toggle = false;


        public MainWindow()
        {
            InitializeComponent();
            serialPort1 = new ReliableSerialPort("COM15", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            serialPort1.OnDataReceivedEvent += SerialPort1_OnDataReceivedEvent;
            //serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();

            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

        }

        private void TimerAffichage_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
           // if (robot.receivedText != "")
           //     textboxRéception.Text += robot.receivedText;
//robot.receivedText = "";

            while (robot.byteListReceived.Count>0)
            {
                textboxRéception.Text += robot.byteListReceived.Dequeue().ToString();

            }
        }

        //private void SerialPort1_DataReceived(object sender, DataReceivedArgs e){}


        private void SerialPort1_OnDataReceivedEvent(object sender, DataReceivedArgs e)
        {
            //textboxRéception.Text += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);

            for (int i = 0; i < e.Data.Length; i++)
            {
                byte b = e.Data[i];
                
                robot.byteListReceived.Enqueue(b);
            }


        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {

            if (toggle == true)

                buttonTest.Background = Brushes.RoyalBlue;
            else
                buttonTest.Background = Brushes.Gold;

            toggle = !toggle;

            ConstruireTableau();
        }


        void ConstruireTableau()
        {
            byte[] byteList = new byte[20];
            for (int i = 0; i < 20; i++)
            {
                byteList[i] = (byte)(2* i);
            }
            serialPort1.Write(byteList, 0, byteList.Length);
        }
















        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {

            if (toggle == true)

                buttonEnvoyer.Background = Brushes.RoyalBlue;
            else
                buttonEnvoyer.Background = Brushes.Gold;

            toggle = !toggle;

            SendMessage();
        }

        private void buttonSupprimer_Click(object sender, RoutedEventArgs e)
        {

            if (toggle == true)

                buttonSupprimer.Background = Brushes.RoyalBlue;
            else
                buttonSupprimer.Background = Brushes.Gold;

            toggle = !toggle;

            ClearMessage();
        }

        void ClearMessage()
        {
            if (textboxRéception.Text != "")
                textboxRéception.Text = "";
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
    