using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp {

    class Logger {

        private bool isClient;

        public Logger(bool isClient) {
            this.isClient = isClient;
        }

        public void Log(string log) {
            if (isClient) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[CLIENT] ");
            }
            else {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[SERVER] ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log);
        }

        public void Exception(string log) {
            Console.ForegroundColor = ConsoleColor.Red;
            if (isClient) {
                Console.Write("[CLIENT EXCEPTION] ");
            }
            else {
                Console.Write("[SERVER EXCEPTION] ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log);
        }

    }
}
