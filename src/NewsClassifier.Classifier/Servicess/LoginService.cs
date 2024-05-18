using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;

namespace NewsClassifier.Classifier.Servicess
{
    public class LoginService
    {
        private readonly string connectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=29092002";

        public void AddUser(long id, string password)
        {
            string hashedPassword = HashPassword(password);

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("INSERT INTO admins (id, password) VALUES (@id, @password)", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("password", hashedPassword);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool AuthenticateUser(long id, string password)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT password FROM admins WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        if(string.IsNullOrEmpty(password))
                            return false;
                        string hashedPasswordFromDb = reader.GetString(0);
                        return VerifyPassword(password, hashedPasswordFromDb);
                    }
                }
            }
            return false;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        public bool UserExists(long id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM admins WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }
    }
}
