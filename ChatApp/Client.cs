﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatApp {
    class Client {

        private Socket connection;

        /* for debugging
        private readonly Logger client_logger = new Logger(true);
        private readonly Logger server_logger = new Logger(false);
         */

        private string lastError = null;

        private readonly List<string> users = new List<string>();
        private readonly MainWindow controller;
        private bool loggedIn;

        public Client(MainWindow mainWindow) {
            this.controller = mainWindow;
        }

        public bool Connect(string host, int port) {
            bool connected = false;
            byte[] bytes = new byte[1024];

            IPHostEntry _host = null;
            IPAddress ipAddress = null;
            IPEndPoint remoteEP = null;

            try {
                _host = Dns.GetHostEntry(host);
                ipAddress = _host.AddressList[0];
                remoteEP = new IPEndPoint(ipAddress, port);
                connection = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                connection.Connect(remoteEP);
                connected = true;
            }
            catch (SocketException e) {
                //client_logger.Exception("Connection to server failed : " + e.Message);
                controller.SetLogText("Connection to server failed : " + e.Message);
                lastError = e.Message;
                connected = false;
            }

            if (connected) {
                //client_logger.Log("Connected to server : " + connection.RemoteEndPoint.ToString());
                controller.SetLogText("Connected to server : " + connection.RemoteEndPoint.ToString());
                StartListenThread();
            }

            return connected;
        }

        public void Disconnect() {
            if (IsConnectionActive()) {
                //client_logger.Log("Diconnecting from host");
                connection.Close();
                connection = null;
            }
        }

        public List<string> Users {
            get { return users; }
        }

        public bool IsConnectionActive() {
            return connection != null;
        }

        public bool SendCommand(string command) {
            if (IsConnectionActive()) {
                byte[] msg = Encoding.ASCII.GetBytes(command + "\n");
                try {
                    connection.Send(msg);
                    return true;
                }
                catch (Exception e) {
                    //server_logger.Exception("Sending command failed : " + e.Message);
                    controller.SetLogText("Sending command failed : " + e.Message);
                    return false;                
                }
            }
            else {
                lastError = "Command not sendt, Connection not active";
                //client_logger.Exception(lastError);
                controller.SetLogText(lastError);
                return false;
            }
        }

        public bool SendPublicMessage(string message) {
            string command = "msg " + message;
            bool success = SendCommand(command);
            controller.UpdateMessagePanel(new TextMessage("You", false, message));
            if (!success) {
                //server_logger.Exception("Public not sendt : " + lastError);
                controller.SetLogText("Public not sendt : " + lastError);
            }

            return success;
        }

        public bool TryLogin(string username) {
            string command = "login " + username;
            SendCommand(command);
            //wait for response
            Thread.Sleep(500);

            //this bool is updated in the handleResonse method that is activly
            //checking for responses like loginok on another thread, thats why
            //we wait with returning this bool 
            return loggedIn;
        }

        public void RefreshUserList() {
            if (IsConnectionActive()) {
                //asking server to respond with the list of users.
                //response is handled in handle method
                SendCommand("users");
            }
        }

        public bool SendPrivateMessage(string recipient, string message) {
            string command = "privmsg " + recipient + " " + message;

            SendCommand(command);

            if (users.Contains(recipient)) {
                controller.UpdateMessagePanel(new TextMessage("You to" + recipient, true, message));
            }
            else {
                controller.SetLogText("Private not sendt : No recipient with this username");
            }

            return users.Contains(recipient);
        }

        public string GetLastError() {
            return (lastError != null) ? lastError : "";
        }

        public void OnMessageRecieved(TextMessage message) {
            controller.UpdateMessagePanel(message);
        }

        private void HandleResponse() {
            try {
                byte[] bytes = new byte[1024];
                int bytesRec = connection.Receive(bytes);
                string response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                string[] info = response.Split(' ');

                //passes first command into switch -> <command> <info> <info> .... \n
                switch (info[0]) {
                    case "msg":
                        HandleMessage(info, false);
                        break;

                    case "privmsg":
                        HandleMessage(info, true);
                        break;

                    case "users":
                        //<command> <user 1> <user 2> ... <user n>\n
                        for (int i = 1; i < info.Length; i++) { users.Add(info[i]); }
                        break;

                    case "cmderr":
                        lastError = "Command error : command not supported";
                        break;

                    case "msgerror":
                        lastError = "Public message error : " + response.Substring(8);
                        break;

                    case "msgerr":
                        lastError = "Private message error : " + response.Substring(5);
                        break;

                    case "loginerr":
                        loggedIn = false;
                        lastError = "Login error : " + response.Substring(8);
                        break;

                    case "loginok\n":
                        loggedIn = true;
                        break;

                    default:
                        lastError = "Command error";
                        break;
                }
            }
            catch (Exception e) {
                //client_logger.Exception("No response from server : " + e.Message);
                controller.SetLogText("No response from server : " + e.Message);
            }
        }

        private void HandleMessage(string[] info, bool priv) {
            //<command> <user> <msg> ... \n
            string fullMessage = "";
            for (int i = 2; i < info.Length; i++) {
                fullMessage += info[i] + " ";
            }
            OnMessageRecieved(new TextMessage(info[1], priv, fullMessage));
        }

        public void StartListenThread() {
            //call parseIncomingCommands() in the new thread.
            Thread parseCaller = new Thread(new ThreadStart(this.ParseIncomingCommand));
            parseCaller.Start();
        }

        public void ParseIncomingCommand() {
            while (IsConnectionActive()) {
                HandleResponse();
            }
        }

    }
}
