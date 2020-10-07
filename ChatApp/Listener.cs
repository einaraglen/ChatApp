using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp {
    class Listener {
        //GUI controller
        private readonly MainWindow controller;

        public Listener(MainWindow mainWindow) {
            this.controller = mainWindow;
        }

        public void OnConnection(string host) {
            this.controller.SetLogText("Connected to server : " + host);
        }

        public void OnLoginResult(bool success, String errMsg) {
            if (!success) {
                this.controller.SetLogText("Login Error : " + errMsg);
            }
        }

        public void OnMessageReceived(TextMessage message) {
            this.controller.UpdateMessagePanel(message);
        }

        public void OnMessageSend(TextMessage message) {
            this.controller.UpdateMessagePanel(message);
        }

        public void OnException(string exception) {
            this.controller.SetLogText(exception);
        }

        public void OnMessageError(String errMsg) {
            this.controller.SetLogText(errMsg);
        }

        public void OnUserList(User[] users) {
            this.controller.UpdateUserList(users);
        }

        public void OnSupportedCommands(String commands) {
            this.controller.SetLogText("Server supports commands : " + commands.Substring(10));
        }

        public void onCommandError(String errMsg) {
            this.controller.SetLogText("Command error : " + errMsg);
        }
    }
}
