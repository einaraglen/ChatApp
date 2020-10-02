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
using System.Text.RegularExpressions;

namespace ChatApp {

    //GUI Controller
    public partial class MainWindow : Window {

        private Client client = new Client();

        public MainWindow() {
            InitializeComponent();
            AddEventListeners();
        }

        private void AddEventListeners() {
            send.Click += Send_Click;
            connect.Click += Connect_Click;
        }

        private void Send_Click(object sender, RoutedEventArgs e) {
            Console.WriteLine("sup");
        }

        private void Connect_Click(object sender, RoutedEventArgs e) {
            if (client.IsConnectionActive()) {
                client.Disconnect();
                connect.Content = "Connect";

                logText.Content = "Disconnected from host";
            }
            else if (address.Text.Trim(' ') != "" && port.Text.Trim(' ') != "") {
                client.Connect(address.Text.Trim(' '), Int32.Parse(port.Text.Trim(' ')));

                connect.Content = "Disconnect";
            }

            if (!client.IsConnectionActive()) {
                logText.Content = client.GetLastError();
            }
        }

        //Makes Port only accept numbers
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
