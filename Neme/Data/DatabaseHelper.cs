 using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Neme.Models;

namespace Neme.Data
{
    public class DatabaseHelper
    {
        private static readonly string DatabaseFile = "NemeApp.db"; // Renamed for broader use
        private static readonly string ConnectionString = $"Data Source={DatabaseFile};Version=3;";

        /// <summary>
        /// Ensures all required tables exist.
        /// </summary>
        public static void InitializeDatabase()
        {
            if (!File.Exists(DatabaseFile))
            {
                SQLiteConnection.CreateFile(DatabaseFile);
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // Create tables if they don't exist
                CreateChatMessagesTable(connection);
                CreatePeersTable(connection);
                CreateTasksTable(connection);
                CreateUsersTable(connection);
            }

           
           
        }

        private static void CreateUsersTable(SQLiteConnection connection)
        {
            string query = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE,
                    PasswordHash TEXT,
                    Email TEXT,
                    Department TEXT,
                    AvatarPath TEXT
            );
        ";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates the ChatMessages table if it doesn't exist.
        /// </summary>
        private static void CreateChatMessagesTable(SQLiteConnection connection)
        {
            string query = @"
                CREATE TABLE IF NOT EXISTS ChatMessages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Content TEXT,
                    IsSentByCurrentUser BOOLEAN,
                    Timestamp TEXT,
                    SenderName TEXT,
                    ReceiverName TEXT,
                    AvatarImage TEXT,
                    DeliveryStatus INTEGER,
                    FilePath TEXT,
                    Type INTEGER
                );";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates the Peers table if it doesn't exist.
        /// </summary>
        private static void CreatePeersTable(SQLiteConnection connection)
        {
            string query = @"
                CREATE TABLE IF NOT EXISTS Peers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Address TEXT UNIQUE NOT NULL,
                    LastSeen TEXT
                );";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates the Tasks table for Kanban board integration.
        /// </summary>
        private static void CreateTasksTable(SQLiteConnection connection)
        {
            string query = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    TaskId TEXT PRIMARY KEY,
                    TaskName TEXT NOT NULL,
                    Status INTEGER NOT NULL,  -- 0 = Initiated, 1 = In Progress, 2 = Completed, 3 = Overdue
                    StartDate TEXT NOT NULL,
                    EndDate TEXT NOT NULL,
                    AssignedTo TEXT  -- Stores peer names or IDs (comma-separated if multiple)
                );";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Returns a connection to the database.
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public static void AddUser(User user)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = @"
            INSERT INTO Users (Username, PasswordHash, Email, Department, AvatarPath) 
            VALUES (@Username, @PasswordHash, @Email, @Department, @AvatarPath)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Department", user.Department);
                    command.Parameters.AddWithValue("@AvatarPath", user.AvatarPath ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
