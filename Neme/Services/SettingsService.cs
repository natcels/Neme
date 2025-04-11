using Neme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;


namespace Neme.Services
{
   public static class SettingsService
    {
        private static readonly string SettingsFile = "usersettings.json";
  public static void SaveSettings(UserSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsFile, json);
    }
 public static UserSettings LoadSettings()
    {
        if (!File.Exists(SettingsFile))
            return new UserSettings(); // return defaults

        var json = File.ReadAllText(SettingsFile);
        return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
    }
    }
}

