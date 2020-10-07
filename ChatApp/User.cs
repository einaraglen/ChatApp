using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ChatApp {
    class User {
        private string username;
        private SolidColorBrush screenColor;

        public User(string username) {
            this.username = username;

            byte[] color = {0, 0, 0};
            color[new Random().Next(0, 2)] = (byte)new Random().Next(100, 255);
            this.screenColor = new SolidColorBrush(Color.FromRgb(0, color[1], color[2]));
        }

        public string Name {
            get { return this.username; }
        }

        public SolidColorBrush ScreenColor {
            get { return this.screenColor; }
        }
    }
}
