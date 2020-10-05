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
using System.ComponentModel;

namespace ChatApp {

    //GUI controller
    public partial class MainWindow : Window {

        private Client client;
        private bool loginOpen = false;
        private bool authorized = false;

        public MainWindow() {
            InitializeComponent();
            AddEventListeners();
            InitTimer();

            //passes controller to client as listener
            client = new Client(this);
        }

        public void Window_Closing(object sender, CancelEventArgs e) {
            //closes client socket on program exit
            client.Disconnect();
        }

        private void AddEventListeners() {
            send.Click += Send_Click;
            connect.Click += Connect_Click;
            authorize.Click += Authorize_Click;
        }

        private void Connect_Click(object sender, RoutedEventArgs e) {
            if (client.IsConnectionActive()) {
                //disconnect from server
                client.Disconnect();
                connect.Content = "Connect";
                send.IsEnabled = false;
                authorize.IsEnabled = false;

                authorized = false;

                logText.Content = "Disconnected from host";
            }
            else if (address.Text.Trim(' ') != "" && port.Text.Trim(' ') != "") {
                //connect to server
                if (client.Connect(address.Text.Trim(' '), Int32.Parse(port.Text.Trim(' ')))) {
                    connect.Content = "Disconnect";
                    send.IsEnabled = true;
                    authorize.IsEnabled = true;
                }
                else {
                    logText.Content = "Connection failed";
                    client.Disconnect();
                }
            }

            if (!client.IsConnectionActive()) {
                logText.Content = client.GetLastError();
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e) {
            if (client.IsConnectionActive() && message.Text.Trim(' ') != "") {

                string[] info = message.Text.Split(' ');

                if (info[0].Equals("/private")) {
                    string text = "";
                    string recipient = info[1];

                    for (int i = 2; i < info.Length; i++) {
                        text += info[i] + " ";
                    }

                    if (client.SendPrivateMessage(recipient, text)) {
                        logText.Content = "";
                    }
                }
                else {
                    client.SendPublicMessage(message.Text);
                    logText.Content = "";
                }

                message.Text = "";
            }
            else {
                logText.Content = "Connection not active or faulty input";
            }
        }

        public void SetLogText(string text) {
            //dispatcher is ised to update gui elements from other
            //threads as this can cause exception
            this.Dispatcher.Invoke(() => {
                logText.Content = text;
            });
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

            try {
                this.Dispatcher.Invoke(() => {
                    userPanel.Children.Clear();

                    if (users.Count != 0) {
                        foreach (string user in users.ToArray()) {
                            Label label = new Label();
                            label.Content = user;

                            //creates menuitem for right click on user list
                            label.ContextMenu = new ContextMenu();
                            MenuItem item = new MenuItem();
                            item.Header = "Private message to " + user;
                            //set tag so we can retreve user when clicked
                            item.Tag = user;
                            item.Click += Item_Click;
                            label.ContextMenu.Items.Add(item);

                            userPanel.Children.Add(label);
                        }

                        users.Clear();
                    }
                });
            }
            catch (Exception exc) {
                logText.Content = exc.Message;
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e) {
            //get the name from the sender, really hack code but found no other way
            string userName = ((MenuItem)sender).Tag.ToString();
            message.Text = "/private " + userName + " ";
        }


        public void UpdateMessagePanel(TextMessage message) {
            this.Dispatcher.Invoke(() => {
                Label label = new Label();
                label.Content = message.ToString();
                Panel panel = message.Private ? privatePanel : messagePanel;
                panel.Children.Add(label);
            });
        }

        

        private void Authorize_Click(object sender, RoutedEventArgs e) {
            if (!authorized) {
                if (loginOpen) {

                    if (username.Text == "") {
                        logText.Content = "Username not allowed";
                    }
                    else {
                        bool success = client.TryLogin(username.Text);

                        if (success) {
                            logText.Content = "Login successful";
                            authorized = true;
                        }
                        else {
                            logText.Content = "Login failed : username taken";
                        }
                    }

                    loginRow.Height = new GridLength(0);
                    authorize.Content = "Authorize";
                    loginOpen = false;
                }
                else {
                    loginRow.Height = new GridLength(60);
                    authorize.Content = "Try";
                    loginOpen = true;
                }
            }
        }

        //makes Port only accept numbers
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }

}
