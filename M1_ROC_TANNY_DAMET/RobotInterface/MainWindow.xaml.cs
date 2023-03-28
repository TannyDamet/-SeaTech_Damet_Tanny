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
        public MainWindow()
        {
            InitializeComponent();
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
            string messageEmis = textboxEmission.Text;
            textboxEmission.Text = "";
            textboxRéception.Text += "\nRéçu : " + messageEmis;
        }
    }

        //  private void TextBoxEmission_KeyUp(KeyEventArgs)
        //  {

        //      if (e.Key == Key.Enter)
        //      {
        //          SendMessage();
        //      }



    }


    