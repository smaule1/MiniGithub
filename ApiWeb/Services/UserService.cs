using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using ApiWeb.Models;
using System.Text.Json;
using DnsClient.Protocol;
using System.Text.RegularExpressions;

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
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(e, emailPattern);
        }

        private bool isValidPassword(string p)
        {
            if (string.IsNullOrEmpty(p)) return false;
            if (p.Length < 8) return false;

            bool hasUpper = p.Any(char.IsUpper);
            bool hasLower = p.Any(char.IsLower);
            bool hasDigit = p.Any(char.IsDigit);
            bool hasSpecial = p.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private bool isValidName(string n)
        {
            if (string.IsNullOrEmpty(n)) return false;

            if (n.Length < 2 || n.Length > 16) return false;

            return n.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
        }

        public IEnumerable<User?> GetUsers()
        {
            var db = Connection.GetDatabase();
            var server = db.Multiplexer.GetServer("localhost", 6379);
            var keys = server.Keys();

            return keys.Select(k =>
            {
                if (k.ToString().StartsWith("session:"))
                {
                    return null;
                }

                var value = db.StringGet(k);

                if (value.IsNullOrEmpty)
                {
                    return null;
                }

                return JsonSerializer.Deserialize<User>(value);
            }).Where(user => user != null);
        }

        public void CreateUser(string email, string password, string name)
        {
            var db = Connection.GetDatabase();

            if (db.KeyExists("user:" + email))
            {
                throw new InvalidOperationException("Ya existe un usuario con el email ingresado.");
            }
            if (!isValidEmail(email))
            {
                throw new ValidationException("El email ingresado no es válido.");
            }
            if (!isValidPassword(password))
            {
                throw new ValidationException($"La contraseña debe cumplir con los requisitos:<br>" +
                    $"1. Mínimo 8 caracteres.<br>" +
                    $"2. Mínimo una letra mayúscula.<br>" +
                    $"3. Mínimo una letra minúscula.<br>" +
                    $"4. Mínimo un número.<br>" +
                    $"5. Mínimo un caracter especial.");
            }
            if (!isValidName(name))
            {
                throw new ValidationException($"El nombre debe cumplir con los requisitos:<br>" +
                    $"1. De 2 a 16 caracteres.<br>" +
                    $"2. Solo debe contener letras, números, '-' o '_'.");
            }

            if (password != string.Empty && name != string.Empty && email != string.Empty)
            {
                User user = new User(email, password, name);
                string userString = JsonSerializer.Serialize(user);
                db.StringSet("user:" + user.Email, userString);
            } else
            {
                throw new InvalidOperationException("Al menos un campo requerido está vacío.");
            }
        }

        public User? GetUser(string email)
        {
            if (!isValidEmail(email))
            {
                throw new ValidationException("El email ingresado no es válido..");
            }

            var db = Connection.GetDatabase();

            string? userString = db.StringGet("user:" + email);

            if (userString == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<User>(userString);
        }

        public void EditUser(string email, string name, string password, string oldPassword)
        {
            var db = Connection.GetDatabase();

            if (db.KeyExists("user:" + email))
            {
                var userString = db.StringGet("user:" + email);
                User? user = JsonSerializer.Deserialize<User>(userString);

                //Validation
                if(User.HashPassword(oldPassword, email) != user.Password)
                {
                    throw new ValidationException("La contraseña introducida no es válida.");
                }
                
                if(name == "" && password == "")
                {
                    throw new InvalidOperationException("Debe haber mínimo un cambio.");
                }

                if (!isValidPassword(password) && password != "")
                {
                    throw new ValidationException($"La contraseña debe cumplir con los requisitos:<br>" +
                    $"1. Mínimo 8 caracteres.<br>" +
                    $"2. Mínimo una letra mayúscula.<br>" +
                    $"3. Mínimo una letra minúscula.<br>" +
                    $"4. Mínimo un número.<br>" +
                    $"5. Mínimo un caracter especial.");
                }
                if (!isValidName(name) && name != "")
                {
                    throw new ValidationException($"El nombre debe cumplir con los requisitos:<br>" +
                    $"1. De 2 a 16 caracteres.<br>" +
                    $"2. Solo debe contener letras, números, '-' o '_'.");
                }

                if (user != null)
                {
                    string newUserString;
                    User newUser;
                    if (password != string.Empty && name != string.Empty)
                    {
                        newUser = new User(user.Email, password, name, user.Id, true);
                        newUserString = JsonSerializer.Serialize(newUser);

                        db.StringSet("user:" + email, newUserString);
                    } else if (password != string.Empty)
                    {
                        newUser = new User(user.Email, password, user.Name, user.Id, true);
                        newUserString = JsonSerializer.Serialize(newUser);

                        db.StringSet("user:" + email, newUserString);
                    } else if (name != string.Empty)
                    {
                        newUser = new User(user.Email, user.Password, name, user.Id);
                        newUserString = JsonSerializer.Serialize(newUser);

                        db.StringSet("user:" + user.Email, newUserString);
                    } else
                    {
                        throw new InvalidOperationException("Se intentó actualizar con campos vacíos.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Intento de actualización sin autorización.");
                }
            }
            else
            {
                throw new InvalidOperationException("El usuario solicitado no existe.");
            }
        }

        public void DeleteUser(string email)
        {
            var db = Connection.GetDatabase();

            if (!isValidEmail(email))
            {
                throw new ValidationException("El email ingresado no es válido..");
            }

            if (db.KeyExists("user:" + email))
            {
                db.KeyDelete("user:" + email);
            } else
            {
                throw new InvalidOperationException("El email no pertenece a ningún usuario existente.");
            }
        }
    }
}
