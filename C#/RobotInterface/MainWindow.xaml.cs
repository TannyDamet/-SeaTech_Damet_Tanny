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
                DecodeMessage(robot.byteListReceived.Dequeue());
                //textboxRéception.Text += robot.byteListReceived.Dequeue().ToString();

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




            string payloadMessage = "Bonjour!";
            byte[] payload = Encoding.ASCII.GetBytes(payloadMessage);

            UartEncodeAndSendMessage(0x0080, payload.Length, payload);

            ProcessDecodedMessage(0x0080, payload.Length, payload);

            //textboxEmission.Clear();
            textboxRéception.Text += "" + payload.Length;
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


        byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            byte checksum = 0;
            checksum ^= 0xFE;

            checksum ^= (byte)(msgFunction >> 8);
            checksum ^= (byte)(msgFunction >> 0);
            checksum ^= (byte)(msgPayloadLength >> 8);
            checksum ^= (byte)(msgPayloadLength >> 0);

            for (int i = 0; i < msgPayload.Length; i++)
            {
                checksum ^= msgPayload[i];
            }

            return checksum;
        }

        void UartEncodeAndSendMessage(int msgFunction,int msgPayloadLength, byte[] msgPayload)
        {

            //Créer le buffer pour la trame complète

            int messageLength = msgPayloadLength + 6;

            byte[] messageBuffer = new byte[messageLength];

            int Pos = 0;

            messageBuffer[Pos++] = 0xFE;
            messageBuffer[Pos++] = (byte)(msgFunction >> 8);
            messageBuffer[Pos++] = (byte)(msgFunction >> 0);
            messageBuffer[Pos++] = (byte)(msgPayloadLength >> 8);
            messageBuffer[Pos++] = (byte)(msgPayloadLength >> 0);

            for (int i = 0; i < msgPayload.Length; i++)
            {
                messageBuffer[Pos++] = msgPayload[i];
            }


            //    // Calculer le checksum
            byte checksum = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload);

            // Ajouter le checksum
            messageBuffer[Pos++] = checksum;

            // Envoyer la trame sur la liaison série
            serialPort1.Write(messageBuffer, 0, messageLength);

        }

        public enum StateReception
        {
            Waiting,
            FunctionMSB,

            FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }

        StateReception rcvState = StateReception.Waiting;
        int msgDecodedFunction = 0;
        int msgDecodedPayloadLength = 0;
        byte[] msgDecodedPayload;
        int msgDecodedPayloadIndex = 0;

        byte receivedChecksum = 0;

        private void DecodeMessage(byte c)
        {
            switch (rcvState)
            {
                case StateReception.Waiting:
                    if (c == 0xFE)
                        rcvState = StateReception.FunctionMSB;
                break;
                case StateReception.FunctionMSB:
                    msgDecodedFunction = c >> 8;
                    rcvState = StateReception.FunctionLSB;
                break;
                case StateReception.FunctionLSB:
                    msgDecodedFunction += c >> 0;
                    rcvState = StateReception.PayloadLengthMSB;
                break;
                case StateReception.PayloadLengthMSB:
                    msgDecodedPayloadLength += c >> 8;
                    rcvState = StateReception.PayloadLengthLSB;
                break;
                case StateReception.PayloadLengthLSB:
                    msgDecodedPayloadLength += c >> 0;
                    if (msgDecodedPayloadLength == 0)
                        rcvState = StateReception.CheckSum;
                    else
                    {
                        rcvState = StateReception.Payload;
                        msgDecodedPayload = new byte[msgDecodedPayloadLength];
                        msgDecodedPayloadIndex = 0;
                    }
                        
                break;
                case StateReception.Payload:
                    msgDecodedPayload[msgDecodedPayloadIndex] = c;
                    msgDecodedPayloadIndex++;
                    if (msgDecodedPayloadIndex == msgDecodedPayloadLength)
                        rcvState = StateReception.CheckSum;
                    else
                        rcvState = StateReception.Payload;

                    break;
               case StateReception.CheckSum:
                    receivedChecksum = c;
                    byte calculatedChecksum = CalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    if (calculatedChecksum == receivedChecksum)
                    {
                        //Success, on a un message valide
                    }
                    rcvState = StateReception.Waiting;

                    break;
                default:
                    rcvState = StateReception.Waiting;
                break;
            }
        }


        public enum MessageFunction
        {
            TexteMessage = 0x0080,
            ReglageLED = 0x0020,
            DistanceTelemetre = 0x0030,
            VitesseMoteur = 0x0040,
        }


        void ProcessDecodedMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            switch (msgFunction)
            {
                case (int)MessageFunction.TexteMessage:

                    for (int i = 0; i < msgPayloadLength; i++)
                    {
                        textboxRéception.Text += "0x" + msgPayload[i].ToString("X") + " ";
                    }
                    textboxRéception.Text += Environment.NewLine;
                    textboxRéception.Text += "Texte reçu : " + Encoding.ASCII.GetString(msgPayload);
                    textboxRéception.Text += Environment.NewLine;
                    break;

                case (int)MessageFunction.ReglageLED:

                    for (int i = 0; i < msgPayloadLength; i++)
                    {
                        textboxRéception.Text += "0x" + msgPayload[i].ToString("X") + " ";
                    }
                    textboxRéception.Text += Environment.NewLine;
                    textboxRéception.Text += "LED Number : " + msgPayload[0].ToString() + " ";
                    if (msgPayload[1] == 1)
                    {
                        textboxRéception.Text += "( Allumée )";
                        checkBoxLed1.IsChecked = true;
                    }
                    else
                        textboxRéception.Text += "( éteinte )";
                    textboxRéception.Text += Environment.NewLine;
                    break;

                case (int)MessageFunction.DistanceTelemetre:

                    for (int i = 0; i < msgPayloadLength; i++)
                    {
                        textboxRéception.Text += "0x" + msgPayload[i].ToString("X") + " ";
                    }
                    textboxRéception.Text += Environment.NewLine;
                    textboxRéception.Text += "Télémètre Gauche : " + msgPayload[0].ToString() + Environment.NewLine;
                    textboxRéception.Text += "Télémètre Centre : " + msgPayload[1].ToString() + Environment.NewLine;
                    textboxRéception.Text += "Télémètre Droit : " + msgPayload[2].ToString() + Environment.NewLine;
                    textboxRéception.Text += Environment.NewLine;

                    IR_Gauche.Content = msgPayload[0];
                    IR_Centre.Content = msgPayload[1];
                    IR_Droit.Content = msgPayload[2];
                    break;

                case (int)MessageFunction.VitesseMoteur:

                    for (int i = 0; i < msgPayloadLength; i++)
                    {
                        textboxRéception.Text += "0x" + msgPayload[i].ToString("X") + " ";
                    }
                    textboxRéception.Text += Environment.NewLine;
                    textboxRéception.Text += "Moteur Gauche : " + msgPayload[0].ToString() + Environment.NewLine;
                    textboxRéception.Text += "Moteur Droit : " + msgPayload[1].ToString() + Environment.NewLine;
                    textboxRéception.Text += Environment.NewLine;


                    Moteur_Gauche.Content = msgPayload[0];
                    Moteur_Droit.Content = msgPayload[1];

                    break;


                default:
                    textboxRéception.Text += "Message inconnu";
                    break;
            }

            
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










