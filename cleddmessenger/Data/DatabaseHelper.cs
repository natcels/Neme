using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Data
{
    public class DatabaseHelper
    {
        private static readonly string DatabaseFile = "ChatMessages.db";
        private static readonly string ConnectionString = $"Data Source={DatabaseFile};Version=3;";

        // Ensure the database and table exist
        public static void InitializeDatabase()
        {
            if (!File.Exists(DatabaseFile))
            {
                SQLiteConnection.CreateFile(DatabaseFile);
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string createTableQuery = @"
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
                        );
                    ";
                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}
