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

namespace ChatApp {

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();

            Client client = new Client();

            client.Connect("datakomm.work", 1300);

            //this works
            //client.SendCommand("5+5\n");

            client.SendCommand("msg hello world\n");

            client.Disconnect();

        }

    }
}
