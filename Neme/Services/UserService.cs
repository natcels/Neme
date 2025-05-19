using System;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using Neme.Models;
using Neme.Data;

namespace Neme.Services
{
    public static class UserService
    {
        public static void AddUser(User user)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string insertQuery = @"
                    INSERT INTO Users (Username, PasswordHash, Email, Department, AvatarPath) 
                    VALUES (@Username, @PasswordHash, @Email, @Department, @AvatarPath)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", HashPassword(user.Password));
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Department", user.Department);
                    command.Parameters.AddWithValue("@AvatarPath", user.AvatarPath ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static User GetUser(string username)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Users WHERE Username = @Username LIMIT 1";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Email = reader["Email"].ToString(),
                                Department = reader["Department"].ToString(),
                                AvatarPath = reader["AvatarPath"].ToString()
                            };
                        }
                    }
                }
            }
            return null; // User not found
        }

        public static bool ValidateUser(string username, string password)
        {
            var user = GetUser(username);
            if (user == null) return false;

            return VerifyPassword(password, user.Password);
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static bool VerifyPassword(string inputPassword, string storedHash)
        {
            return HashPassword(inputPassword) == storedHash;
        }

        public static bool UserExists(string username)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
