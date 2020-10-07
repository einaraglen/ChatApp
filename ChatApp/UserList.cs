using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp {
    class UserList {
        private List<User> users;

        public UserList() {
            users = new List<User>();
        }

        public void Add(string name) {
            users.Add(new User(name));
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

        public User Get(string name) {
            foreach (User user in users) {
                if (user.Name.Equals(name)) {
                    return user;
                }
            }

            return null;
        }

        public List<User> Users {
            get { return this.users; }
        }
    }
}
