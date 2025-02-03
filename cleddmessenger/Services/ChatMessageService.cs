using cleddmessenger.Data;
using cleddmessenger.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cleddmessenger.Services
{
    public class ChatMessageService
    {
        public void SaveMessage(ChatMessage message)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string insertQuery = @"
                    INSERT INTO ChatMessages (Content, IsSentByCurrentUser, Timestamp, SenderName, ReceiverName, AvatarImage, DeliveryStatus, FilePath, Type)
                    VALUES (@Content, @IsSentByCurrentUser, @Timestamp, @SenderName, @ReceiverName, @AvatarImage, @DeliveryStatus, @FilePath, @Type);
                ";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Content", message.Content);
                    command.Parameters.AddWithValue("@IsSentByCurrentUser", message.IsSentByCurrentUser);
                    command.Parameters.AddWithValue("@Timestamp", message.Timestamp.ToString("o"));
                    command.Parameters.AddWithValue("@SenderName", message.SenderName);
                    command.Parameters.AddWithValue("@ReceiverName", message.ReceiverName);
                    command.Parameters.AddWithValue("@AvatarImage", message.AvatarImage);  
                    command.Parameters.AddWithValue("@DeliveryStatus", (int)message.DeliveryStatus);
                    command.Parameters.AddWithValue("@FilePath", message.FilePath);
                    command.Parameters.AddWithValue("@Type", (int)message.Type);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveFileMessage(ChatMessage message)
        {
            SaveMessage(message); 
                                  
        }

        public List<ChatMessage> GetMessages(string sender, string receiver)
        {
            var messages = new List<ChatMessage>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string selectQuery = @"
                    SELECT * FROM ChatMessages
                    WHERE (SenderName = @Sender AND ReceiverName = @Receiver)
                       OR (SenderName = @Receiver AND ReceiverName = @Sender)
                    ORDER BY Timestamp ASC;
                ";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Sender", sender);
                    command.Parameters.AddWithValue("@Receiver", receiver);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new ChatMessage
                            {
                                Content = reader["Content"].ToString(),
                                IsSentByCurrentUser = Convert.ToBoolean(reader["IsSentByCurrentUser"]),
                                Timestamp = DateTime.Parse(reader["Timestamp"].ToString()),
                                SenderName = reader["SenderName"].ToString(),
                                ReceiverName = reader["ReceiverName"].ToString(),
                                AvatarImage = reader["AvatarImage"]?.ToString(),
                                DeliveryStatus = (MessageStatus)Convert.ToInt32(reader["DeliveryStatus"]),
                                FilePath = reader["FilePath"]?.ToString(),
                                Type = (MessageType)Convert.ToInt32(reader["Type"])
                            });
                        }
                    }
                }
            }
            return messages;
        }

        public void UpdateMessageStatus(int messageId, MessageStatus status)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string updateQuery = "UPDATE ChatMessages SET DeliveryStatus = @Status WHERE Id = @Id;";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Status", (int)status);
                    command.Parameters.AddWithValue("@Id", messageId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
