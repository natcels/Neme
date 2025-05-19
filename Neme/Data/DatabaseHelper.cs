 using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.EntitySql;
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
                CreateRemindersTable(connection);
                CreateSHaredFIlesTable(connection);
            }
        }

        /// <summary>
        /// Returns a connection to the database.
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        private static void CreateRemindersTable(SQLiteConnection connection)
        {
            string query = @"
            CREATE TABLE IF NOT EXISTS Reminders (
                Id TEXT PRIMARY KEY NOT NULL,
                Title TEXT NOT NULL,
                Notes TEXT,
                ReminderDate TEXT NOT NULL,
                IsCompleted INTEGER NOT NULL DEFAULT 0
                );
                ";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
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
                    Status INTEGER NOT NULL,
                    StartDate TEXT NOT NULL,
                    EndDate TEXT NOT NULL,
                    AssignedTo TEXT 
                );";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

       


        private static void CreateSHaredFIlesTable(SQLiteConnection connection)
        {
            string query = @"
                    CREATE TABLE IF NOT EXISTS SharedFiles (
                        FileID TEXT NOT NULL PRIMARY KEY,
                        FileName TEXT NOT NULL,
                        FilePath TEXT NOT NULL,
                        OwnerUsername TEXT NOT NULL,
                        Collaborators TEXT,
                        UploadedAt TEXT
                    )";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        // Insert Data

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
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Department", user.Department);
                    command.Parameters.AddWithValue("@AvatarPath", user.AvatarPath ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void InsertSharedFile(SharedFile SharedFile)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = @"
                INSERT INTO SharedFiles (FileID, FileName, FilePath, OwnerUsername, Collaborators, UploadedAt)
                VALUES (@FileID, @FileName, @FilePath, @OwnerUsername, @Collaborators, @UploadedAt)
                ";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@FileID", SharedFile.Id.ToString().ToUpper());
                    command.Parameters.AddWithValue("@FileName", SharedFile.FileName);
                    command.Parameters.AddWithValue("@FilePath", SharedFile.FilePath);
                    command.Parameters.AddWithValue("@OwnerUserName", SharedFile.OwnerUsername);
                    command.Parameters.AddWithValue("@Collaborators", SharedFile.Collaborators);

                    command.ExecuteNonQuery ();    
                }
            }
        }

        public static void AddReminder(Reminder reminder)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Reminders (Title, Notes, ReminderDate, IsCompleted) VALUES (@Title, @Notes, @ReminderDate, @IsCompleted)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", reminder.Title);
                    command.Parameters.AddWithValue("@Notes", reminder.Notes);
                    command.Parameters.AddWithValue("@ReminderDate", reminder.ReminderDate.ToString("s")); // ISO format
                    command.Parameters.AddWithValue("@IsCompleted", reminder.IsCompleted ? 1 : 0);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Read Data

        public static List<Reminder> GetAllReminders()
        {
            var reminders = new List<Reminder>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Reminders";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reminders.Add(new Reminder
                        {
                            Id = reader["Id"].ToString(),
                            Title = reader["Title"].ToString(),
                            Notes = reader["Notes"].ToString(),
                            ReminderDate = DateTime.Parse(reader["ReminderDate"].ToString()),
                            IsCompleted = Convert.ToInt32(reader["IsCompleted"]) == 1
                        });
                    }
                }
            }

            return reminders;
        }

        // Update Data

    }
}
