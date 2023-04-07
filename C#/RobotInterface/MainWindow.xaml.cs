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
using System.IO.Ports;


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

        public byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte checksum = 0;

            // Add the function code and payload length to the checksum
            checksum += (byte)msgFunction;
            checksum += (byte)msgPayloadLength;

            // Add each byte of the payload to the checksum
            for (int i = 0; i < msgPayloadLength; i++)
            {
                checksum += msgPayload[i];
            }

            // Return the 2's complement of the checksum
            return (byte)(~checksum + 1);
        }



public void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
    {
        // Calculate the checksum
        byte checksum = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload);

        // Create the message buffer
        byte[] message = new byte[msgPayloadLength + 5];

        // Add the start byte, function code, payload length, payload, and checksum to the message buffer
        message[0] = 0x01; // Start byte
        message[1] = (byte)msgFunction;
        message[2] = (byte)msgPayloadLength;
        Array.Copy(msgPayload, 0, message, 3, msgPayloadLength);
        message[msgPayloadLength + 3] = checksum;

        // Add the end byte to the message buffer
        message[msgPayloadLength + 4] = 0x04; // End byte

        // Open the serial port
        SerialPort serialPort = new SerialPort("COM15", 115200);
        serialPort.Open();

        // Send the message
        serialPort.Write(message, 0, message.Length);

        // Close the serial port
        serialPort.Close();
    }
















    //*****************************************************************************************************************************

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
            string messageEmis = textboxEmission.Text;
            textboxEmission.Text = "";
            textboxRéception.Text += "\nRéçu : " + messageEmis;
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
    