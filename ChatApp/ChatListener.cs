using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp {
    interface ChatListener {

        public void OnDisconnect();

        public void OnLoginResults(bool siccess, string errorMessage);

        public void OnMessageReceived(TextMessage message);

        public void OnMessageError(string errorMessage);

        public void OnUserList(List<string> usernames);

        public void OnCommandError(string errorMessage);
    }
}
