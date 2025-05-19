using Neme.Data;
using Neme.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Neme.Services
{
    public static class FileService
    {

        public static void SaveSharedFile(SharedFile file)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                var cmd = new SQLiteCommand(@"
                    INSERT INTO SharedFiles 
                    (Id, FileName, FilePath, OwnerUsername, Collaborators, UploadedAt)
                    VALUES (@Id, @FileName, @FilePath, @OwnerUsername, @Collaborators, @UploadedAt)", connection);

                cmd.Parameters.AddWithValue("@Id", file.Id.ToString());
                cmd.Parameters.AddWithValue("@FileName", file.FileName);
                cmd.Parameters.AddWithValue("@FilePath", file.FilePath);
                cmd.Parameters.AddWithValue("@OwnerUsername", file.OwnerUsername);
                cmd.Parameters.AddWithValue("@Collaborators", string.Join(",", file.Collaborators));
                cmd.Parameters.AddWithValue("@UploadedAt", file.UploadedAt.ToString("s"));

                cmd.ExecuteNonQuery();
            }
        }

        // Optional helper to get a user's shared files
        public static List<SharedFile> GetFilesByOwner(string username)
        {
            var files = new List<SharedFile>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                var cmd = new SQLiteCommand("SELECT * FROM SharedFiles WHERE OwnerUsername = @Owner", connection);
                cmd.Parameters.AddWithValue("@Owner", username);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        files.Add(new SharedFile
                        {
                            Id = Guid.Parse(reader["Id"].ToString()),
                            FileName = reader["FileName"].ToString(),
                            FilePath = reader["FilePath"].ToString(),
                            OwnerUsername = reader["OwnerUsername"].ToString(),
                            Collaborators = new List<string>((reader["Collaborators"].ToString() ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)),
                            UploadedAt = DateTime.Parse(reader["UploadedAt"].ToString())
                        });
                    }
                }
            }
            return files;
        }

        public static void AddCollaborator(Guid fileId, string collaboratorUsername)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                // Get current collaborators
                var cmd = new SQLiteCommand("SELECT Collaborators FROM SharedFiles WHERE Id = @Id", connection);
                cmd.Parameters.AddWithValue("@Id", fileId.ToString());

                var currentCollaborators = cmd.ExecuteScalar()?.ToString() ?? "";

                var collaboratorsList = new HashSet<string>(currentCollaborators.Split(',', StringSplitOptions.RemoveEmptyEntries));
                collaboratorsList.Add(collaboratorUsername);

                // Update collaborators
                var updateCmd = new SQLiteCommand("UPDATE SharedFiles SET Collaborators = @Collaborators WHERE Id = @Id", connection);
                updateCmd.Parameters.AddWithValue("@Collaborators", string.Join(",", collaboratorsList));
                updateCmd.Parameters.AddWithValue("@Id", fileId.ToString());
                updateCmd.ExecuteNonQuery();
            }
        }

    }
}