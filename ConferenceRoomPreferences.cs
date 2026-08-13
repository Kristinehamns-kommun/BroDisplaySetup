using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BroDisplaySetup
{
    // Persists the user's answer to "is this a conference room?" per external display, keyed by
    // the display's EDID serial number, so the prompt doesn't need to be repeated every time the
    // same screen is docked into. Serial numbers that are blank (some EDIDs don't report one) are
    // never persisted - remembering under a blank key could apply one room's answer to a different
    // screen that also failed to report a serial.
    static class ConferenceRoomPreferences
    {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BroDisplaySetup",
            "conference-rooms.json");

        public static bool? GetPreference(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
            {
                return null;
            }

            Dictionary<string, bool> preferences = Load();
            return preferences.TryGetValue(serial, out bool isConferenceRoom) ? isConferenceRoom : (bool?)null;
        }

        public static void SetPreference(string serial, bool isConferenceRoom)
        {
            if (string.IsNullOrWhiteSpace(serial))
            {
                return;
            }

            Dictionary<string, bool> preferences = Load();
            preferences[serial] = isConferenceRoom;
            Save(preferences);
        }

        public static void ForgetAll()
        {
            Save(new Dictionary<string, bool>());
        }

        private static Dictionary<string, bool> Load()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    return new Dictionary<string, bool>();
                }

                string json = File.ReadAllText(ConfigFilePath);
                return JsonSerializer.Deserialize<Dictionary<string, bool>>(json) ?? new Dictionary<string, bool>();
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException || ex is UnauthorizedAccessException)
            {
                return new Dictionary<string, bool>();
            }
        }

        private static void Save(Dictionary<string, bool> preferences)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));
                string json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // Best-effort persistence - if we can't write the file, the user is just asked again next time.
            }
        }
    }
}
