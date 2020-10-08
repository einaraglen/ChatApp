using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Timers;
using System.ComponentModel;

namespace ChatApp {

    //GUI controller
    public partial class MainWindow : Window {

        private Client client;
        private User[] users;
        private System.Timers.Timer timer;
        private bool loginOpen = false;
        private bool authorized = false;

        public MainWindow() {
            InitializeComponent();
            AddEventListeners();

            //passes controller to listener as communicator for client
            client = new Client(new Listener(this));
            this.users = new User[0];
        }

        public void Window_Closing(object sender, CancelEventArgs e) {
            //closes client socket on program exit
            client.Disconnect();
        }

        private void AddEventListeners() {
            send.Click += Send_Click;
            connect.Click += Connect_Click;
            authorize.Click += Authorize_Click;
            help.Click += Help_Click;
        }

        private void Connect_Click(object sender, RoutedEventArgs e) {
            if (client.IsConnectionActive()) {
                //disconnect from server
                client.Disconnect();
                //reset GUI
                connect.Content = "Connect";
                send.IsEnabled = false;
                message.IsEnabled = false;
                authorize.IsEnabled = false;
                help.IsEnabled = false;
                authorized = false;
                yourname.Content = "";
                messagePanel.Children.Clear();
                privatePanel.Children.Clear();
                userPanel.Children.Clear();
                userCount.Content = "";
                //stop asking for userlist
                StopTimer();
            }
            else if (address.Text.Trim(' ') != "" && port.Text.Trim(' ') != "") {
                //connect to server
                if (client.Connect(address.Text.Trim(' '), Int32.Parse(port.Text.Trim(' ')))) {
                    connect.Content = "Disconnect";
                    send.IsEnabled = true;
                    message.IsEnabled = true;
                    authorize.IsEnabled = true;
                    help.IsEnabled = true;
                    //start refresh timer
                    InitTimer();
                }
                else {
                    logText.Content = "Connection failed";
                    client.Disconnect();
                }
            }
            else {
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

                    if (authorized) {
                        if (client.SendPrivateMessage(recipient, text)) {
                            logText.Content = "";
                        }
                    }
                    else {
                        logText.Content = "Not authorized, Please login";
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
            //dispatcher is used to update gui elements from other
            //threads as this can cause exception
            this.Dispatcher.Invoke(() => {
                logText.Content = text;
            });
        }

        private void InitTimer() {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Tick;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void StopTimer() {
            timer.Stop();
            timer.Dispose();
        }

        public void UpdateUserList(User[] users) {
            this.users = users;
        }

        private void Tick(object sender, ElapsedEventArgs e) {
            client.RefreshUserList();

            this.Dispatcher.Invoke(() => {
                userCount.Content = "Users online : " + this.users.Length;
                userPanel.Children.Clear();

                try {
                    foreach (User user in this.users) {
                        TextBlock text = new TextBlock();
                        WrapPanel wrapper = new WrapPanel();
                        if (user.Name.Length > 13) {
                            text.Text = user.Name.Substring(0, 13) + "..";
                        }
                        else {
                            text.Text = user.Name;
                        }
                        text.FontSize = 15;

                        Rectangle indicator = new Rectangle();
                        indicator.Height = 10;
                        indicator.Width = 10;
                        indicator.Fill = (
                            new SolidColorBrush(Color.FromRgb(user.ScreenColor[0], user.ScreenColor[1], user.ScreenColor[2]))
                        );
                        indicator.HorizontalAlignment = HorizontalAlignment.Left;
                        indicator.Margin = new Thickness(5, 2, 5, 0);

                        //creates menuitem for right click on user list
                        text.ContextMenu = new ContextMenu();
                        MenuItem item = new MenuItem();
                        item.Header = "Private message to " + user.Name;
                        //set tag so we can retreve user when clicked
                        item.Tag = user.Name;
                        item.Click += Item_Click;
                        text.ContextMenu.Items.Add(item);


                        wrapper.Children.Add(indicator);
                        wrapper.Children.Add(text);
                        userPanel.Children.Add(wrapper);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }

            });

        }

        private void Item_Click(object sender, RoutedEventArgs e) {
            //get the name from the sender, given through tag
            string userName = ((MenuItem)sender).Tag.ToString();
            message.Text = "/private " + userName + " ";
        }

        private void Help_Click(object sender, RoutedEventArgs e) {
            client.AskSupportedCommands();
        }


        public void UpdateMessagePanel(TextMessage message) {
            this.Dispatcher.Invoke(() => {
                TextBlock text = new TextBlock();
                text.Text = message.ToString();
                text.FontSize = 15;
                text.TextWrapping = TextWrapping.Wrap;
                text.Margin = new Thickness(20, 0, 0, 0);
                
                Grid wrapper = new Grid();
                wrapper.HorizontalAlignment = HorizontalAlignment.Stretch;

                Rectangle indicator = new Rectangle();
                indicator.Height = 10;
                indicator.Width = 10;
                indicator.HorizontalAlignment = HorizontalAlignment.Left;
                indicator.Margin = new Thickness(5, 2, 5, 0);

                User current = client.Users.Get(message.Sender);
                if (current != null) {
                    indicator.Fill = (
                        new SolidColorBrush(Color.FromRgb(current.ScreenColor[0], current.ScreenColor[1], current.ScreenColor[2]))
                    );
                }
                else {
                   if (message.Sender.StartsWith('Y')) {
                        text.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                        indicator.Fill = (
                            new SolidColorBrush(Color.FromRgb(255, 255, 255))
                        );
                   }
                }

                Panel panel = message.Private ? privatePanel : messagePanel;
                wrapper.Children.Add(indicator);
                wrapper.Children.Add(text);
                panel.Children.Add(wrapper);

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
                            authorize.IsEnabled = false;
                            authorized = true;

                            yourname.Content = "You : " + username.Text;
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
