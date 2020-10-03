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
using System.Timers;

namespace ChatApp {

    //GUI Controller
    public partial class MainWindow : Window {

        private Client client = new Client();

        public MainWindow() {
            InitializeComponent();
            AddEventListeners();
            InitTimer();
        }

        private void AddEventListeners() {
            send.Click += Send_Click;
            connect.Click += Connect_Click;
            authorize.Click += Authorize_Click;
        }

        private void Send_Click(object sender, RoutedEventArgs e) {
            if (client.IsConnectionActive() && message.Text.Trim(' ') != "") {
                client.SendPublicMessage(message.Text);
                logText.Content = "";
            }
            else {
                logText.Content = "Connection not active or faulty input";
            }
        }

        public void InitTimer() {
            Timer timer = new Timer(1000);
            timer.Elapsed += Tick;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void Tick(object sender, ElapsedEventArgs e) {
            client.RefreshUserList();
            List<string> users = client.Users;

            this.Dispatcher.Invoke(() => {
                userPanel.Children.Clear();

                if (users.Count != 0) {
                    foreach (string user in users.ToArray()) {
                        Label label = new Label();
                        label.Content = user;
                        userPanel.Children.Add(label);
                    }

                    users.Clear();
                }
            });
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

        private void Authorize_Click(object sender, RoutedEventArgs e) {

        }

        //Makes Port only accept numbers
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Inherited methods

        public void OnLoginResults(bool success, string errorMessage) {
            if (success) {
                logText.Content = "Login successful";
            }
            else {
                logText.Content = "Login failed";
            }
        }
    }

}
