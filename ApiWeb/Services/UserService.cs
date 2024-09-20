using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using ApiWeb.Models;
using System.Text.Json;

namespace ApiWeb.Services
{
    public class UserService
    {
        [Required]
        private static readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return _lazyConnection.Value;
            }
        }

        static UserService()
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect("localhost")
            );
        }

        private bool isValidEmail(string e)
        {
            return true;
        }

        private bool isValidPassword(string p)
        {
            return true;
        }

        private bool isValidName(string n)
        {
            return true;
        }

        public IEnumerable<User?> GetUsers()
        {
            var db = Connection.GetDatabase();
            var server = db.Multiplexer.GetServer("localhost", 6379);
            var keys = server.Keys();

            return keys.Select(k =>
            {
                var value = db.StringGet(k);

                if (value.IsNullOrEmpty)
                {
                    return null;
                }

                return JsonSerializer.Deserialize<User>(value);
            });
        }

        public void CreateUser(string email, string password, string name)
        {
            var db = Connection.GetDatabase();

            if (db.KeyExists(email))
            {
                throw new InvalidOperationException("An user with this email already exists.");
            }
            if (!isValidEmail(email))
            {
                throw new ValidationException("Email is not valid.");
            }
            if (!isValidPassword(password))
            {
                throw new ValidationException("Password must meet the minimum requirements.");
            }
            if (!isValidName(name))
            {
                throw new ValidationException("Name is not valid.");
            }

            if (password != string.Empty && name != string.Empty && email != string.Empty)
            {
                User user = new User(email, password, name);
                string userString = JsonSerializer.Serialize(user);
                db.StringSet("user:" + user.Email, userString);
            } else
            {
                throw new InvalidOperationException("At least one required field is empty.");
            }
        }

        public User? GetUser(string email)
        {
            if (!isValidEmail(email))
            {
                throw new ValidationException("Email is not valid.");
            }

            var db = Connection.GetDatabase();

            string? userString = db.StringGet("user:" + email);

            if (userString == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<User>(userString);
        }

        public void EditUser(string email, string name, string password)
        {
            var db = Connection.GetDatabase();

            if (db.KeyExists("user:" + email))
            {
                var userString = db.StringGet("user:" + email);
                User? user = JsonSerializer.Deserialize<User>(userString);

                //Validation
                if (!isValidPassword(password) && password != user.Password)
                {
                    throw new ValidationException("Password must meet the minimum requirements.");
                }
                if (!isValidName(name) && name != user.Name)
                {
                    throw new ValidationException("Name is not valid.");
                }

                if (user != null)
                {
                    string newUserString;
                    User newUser;
                    if (password == user.Password && name != string.Empty)
                    {
                        newUser = new User(email, user.Password, name, user.Id);
                        newUserString = JsonSerializer.Serialize(newUser);

                        db.StringSet("user:" + email, newUserString);
                    } else if (user.Name == name && password != string.Empty)
                    {
                        newUser = new User(email, password, user.Name, user.Id, true);
                        newUserString = JsonSerializer.Serialize(newUser);

                        db.StringSet("user:" + email, newUserString);
                    } else if (password != string.Empty && name != string.Empty)
                    {
                        newUser = new User(email, password, name, user.Id, true);
                        newUserString = JsonSerializer.Serialize(newUser);

                        db.StringSet("user:" + email, newUserString);
                    } else
                    {
                        throw new InvalidOperationException("Attempted update with empty fields.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Attempting to modify an user without authorization.");
                }
            }
            else
            {
                throw new InvalidOperationException("Requested user does not exist.");
            }
        }

        public void DeleteUser(string email)
        {
            var db = Connection.GetDatabase();

            if (!isValidEmail(email))
            {
                throw new ValidationException("Email is not valid.");
            }

            if (db.KeyExists("user:" + email))
            {
                db.KeyDelete("user:" + email);
            } else
            {
                throw new InvalidOperationException("Email does not belong to an existing user.");
            }
        }
    }
}
