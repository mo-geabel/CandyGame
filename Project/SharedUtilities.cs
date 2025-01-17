using System;
using System.Collections.Generic;
using System.IO;

public static class HighScoreManager
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "HighScores.txt");

    // Method to load high scores from a text file
    public static List<KeyValuePair<string, int>> LoadHighScores()
    {
        var highScores = new List<KeyValuePair<string, int>>();

        if (!File.Exists(FilePath))
        {
            return highScores;
        }

        try
        {
            foreach (var line in File.ReadAllLines(FilePath))
            {
                var parts = line.Split(':'); // Split the line by the colon delimiter
                if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                {
                    highScores.Add(new KeyValuePair<string, int>(parts[0], score));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading high scores: {ex.Message}");
        }

        return highScores;
    }

    // Method to save high scores to a text file
    public static void SaveHighScores(List<KeyValuePair<string, int>> highScores)
    {
        try
        {
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var writer = new StreamWriter(FilePath))
            {
                foreach (var entry in highScores)
                {
                    writer.WriteLine($"{entry.Key}:{entry.Value}"); // Write each score as Name:Score
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving high scores: {ex.Message}");
        }
    }

}
