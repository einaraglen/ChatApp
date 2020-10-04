using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp {
    class TextMessage {

        private string sender;
        private bool priv;
        private string text;

        public TextMessage(string sender, bool priv, string text) {
            this.sender = sender;
            this.priv = priv;
            this.text = text;

        }

        public string Sender {
            get { return this.sender; }
        }

        public bool Private {
            get { return this.priv; }
        }

        public string Text {
            get { return this.text; }
        }

        override
        public string ToString() {
            return "" + DateTime.Now.ToString("HH:mm") + " " + this.sender + " : " + text;
        }

    }
}
