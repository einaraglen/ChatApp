using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp {
    class Client {

        private Socket connection;

        private Logger client_logger = new Logger(true);
        private Logger server_logger = new Logger(false);

        private string lastError = null;
        private static List<ChatListener> listeners = null;

        public bool Connect(string host, int port) {

            byte[] bytes = new byte[1024];

            try {
                IPHostEntry _host = Dns.GetHostEntry(host);
                IPAddress ipAddress = _host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                connection = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

                connection.Connect(remoteEP);
                
                client_logger.Log("Connected to server : " + connection.RemoteEndPoint.ToString());
                return true;
            }
            catch (SocketException e) {
                client_logger.Exception("Connection to server failed : " + e.Message);
                lastError = e.Message;
                return false;
            }
        }

        public void Disconnect() {
            if (IsConnectionActive()) {
                client_logger.Log("Diconnecting from host");
                connection.Close();
            }
        }

        public bool IsConnectionActive() {
            return connection != null;
        }

        public bool SendCommand(string command) {
            if (IsConnectionActive()) {
                byte[] msg = Encoding.ASCII.GetBytes(command);
                // Send the data through the socket. returns bytes sendt 
                try {
                    connection.Send(msg);

                    //Buffer
                    byte[] bytes = new byte[1024];

                    int bytesRec = connection.Receive(bytes);
                    string response = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    server_logger.Log(response);

                    return true;
                }
                catch(Exception e) {
                    server_logger.Exception("Sending command failed : " + e.Message);
                    return false;                
                }
            }
            else {
                client_logger.Exception("Command not sendt, Connection not active");
                return false;
            }
        }

        public bool SendPublicMessage(string message) {
            //Buffer
            byte[] bytes = new byte[1024];
            string command = "msg " + message + "\n";

            bool success = SendCommand(command);

            int bytesRec = connection.Receive(bytes);
            string response = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            if (success && response.Contains("msgok")) {
                return true;
            }
            else {
                // removes "msgerror "
                //lastError = response.Substring(8);
                server_logger.Exception("Public not sendt : " + response);
                return false;
            }
        }

        public void TryLogin(string username) {
            //Buffer
            byte[] bytes = new byte[1024];
            string command = "login " + username + "\n";

            bool success = SendCommand(command);

            int bytesRec = connection.Receive(bytes);
            string response = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            if (success && response.Contains("loginok")) {
                //return true;
            }
            else {
                // removes "loginerr "
                //lastError = response.Substring(8);
                server_logger.Exception("Try login failed : " + response);
            }
        }

        public void RefreshUserList() {

        }

        public bool SendPrivateMessage(string recipient, string message) {
            //Buffer
            byte[] bytes = new byte[1024];
            string command = "privmsg " + recipient + " " + message + "\n";

            bool success = SendCommand(command);

            int bytesRec = connection.Receive(bytes);
            string response = Encoding.ASCII.GetString(bytes, 0, bytesRec);

            if (success && response.Contains("msgok 1")) {
                return true;
            }
            else {
                // removes "msgerr "
                //lastError = response.Substring(6);
                server_logger.Exception("Private not sendt : " + response);
                return false;
            }
        }

        public void AskSupportedCommand() {

        }

        public string WaitServerResponse() {
            return null;
        }

        public string GetLastError() {
            if (lastError != null) {
                return lastError;
            }
            else {
                return "";
            }
        }

        public void StartListenThread() {
            // Call parseIncomingCommands() in the new thread.
            Thread parseCaller = new Thread(new ThreadStart(this.ParseIncomingCommand));
            parseCaller.Start();
        }

        public void ParseIncomingCommand() {
            while (IsConnectionActive()) {

            }
        }

        public void AddListener(ChatListener listener) {
            if (!listeners.Contains(listener)) {
                listeners.Add(listener);
            }
        }

        public void RemoveListener(ChatListener listener) {
            listeners.Remove(listener);
        }

        public void OnLoginResult(bool success, string errorMessage) {
            foreach (ChatListener listener in listeners) {
                listener.OnLoginResults(success, errorMessage);
            }
        }

        public void OnDisconnect() {

        }

        private void OnMsgReceived(bool priv, string sender, string text) {

        }

        private void OnMessageError(string errorMessage) {

        }

        private void OnCmdError(string errorMessage) {

        }

        private void OnSupported(List<string> commands) {

        }

    }
}
