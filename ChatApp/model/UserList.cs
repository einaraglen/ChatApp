using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp {
    //basiclly makes it easier for me to add, remove and check for 
    //existing users when updatind. and holds the user colors
    class UserList {
        private List<User> users;

        public UserList() {
            users = new List<User>();
        }

        public void Add(string name) {
            users.Add(new User(name));
        }

        public void Add(List<string> usernames) {
            foreach (string user in usernames) {
                this.Add(user);
            }
        }

        public void Remove(string name) {
            users.Remove(this.Get(name));
        }

        public bool IsEmpty() {
            return users.Count == 0;
        }

        public int Count {
            get { return users.Count; }
        }

        public void Clear() {
            users.Clear();
        }

        public bool Contains(string name) {
            return this.Get(name) != null;
        }

        public User Get(string name) {
            foreach (User user in users) {
                if (user.Name.Equals(name)) {
                    return user;
                }
            }

            return null;
        }

        public User[] ToArray() {
            return this.users.ToArray();
        }

        public List<User> Users {
            get { return this.users; }
        }
    }
}
