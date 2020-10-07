using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ChatApp {
    class User {
        private string username;
        private byte[] screenColor;

        public User(string username) {
            this.username = username;

            this.screenColor = new byte[3];
            this.screenColor[0] = (byte)new Random().Next(0, 255);
            this.screenColor[1] = (byte)new Random().Next(0, 255);
            this.screenColor[2] = (byte)new Random().Next(0, 255);
        }

        public string Name {
            get { return this.username; }
        }

        public byte[] ScreenColor {
            get { return this.screenColor; }
        }
    }
}
