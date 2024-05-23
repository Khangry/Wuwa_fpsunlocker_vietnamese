using Newtonsoft.Json.Linq;
using System;
using System.Data.SQLite;
using System.IO;

namespace Wuwa_fpsunlocker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ASCII Art
            Console.WriteLine(@"
 __          __                             ______           _    _       _            _             
 \ \        / /                            |  ____|         | |  | |     | |          | |            
  \ \  /\  / /   ___      ____ _   ______  | |__ _ __  ___  | |  | |_ __ | | ___   ___| | _____ _ __ 
   \ \/  \/ / | | \ \ /\ / / _` | |______| |  __| '_ \/ __| | |  | | '_ \| |/ _ \ / __| |/ / _ \ '__|
    \  /\  /| |_| |\ V  V / (_| |          | |  | |_) \__ \ | |__| | | | | | (_) | (__|   <  __/ |   
     \/  \/  \__,_| \_/\_/ \__,_|          |_|  | .__/|___/  \____/|_| |_|_|\___/ \___|_|\_\___|_|   
                                                | |                                                  
                                                |_|                                                  
");
            Console.WriteLine("Các bước:");
            Console.WriteLine("1) PHẢI ĐỂ FPS TRONG GAME LÀ 60FPS");
            Console.WriteLine("2) Tắt game");
            Console.WriteLine("3) Tắt Vsync và không được điều chỉnh FPS sau khi chạy file, chỉnh thì sẽ mất 120 fps");
            Console.WriteLine("4) Và thế là xong\n");
            Console.WriteLine("Author: Discord: impots");
            Console.WriteLine("\nNhập đường dẫn đến file SQLite \n (Ví dụ C:\\Wuthering Waves\\Wuthering Waves Game\\Client\\Saved\\LocalStorage\\LocalStorage.db):");
            string dbPath = Console.ReadLine();
            string connectionString = $"Data Source={dbPath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connected to the SQLite database.");

                    // Step 1: Read the GameQualitySetting JSON
                    string selectQuery = "SELECT value FROM LocalStorage WHERE key = 'GameQualitySetting';";
                    string gameQualitySettingJson = null;

                    using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                gameQualitySettingJson = reader["value"].ToString();
                                Console.WriteLine("Original GameQualitySetting JSON:");
                                Console.WriteLine(gameQualitySettingJson);
                            }
                        }
                    }

                    if (gameQualitySettingJson == null)
                    {
                        Console.WriteLine("No GameQualitySetting found.");
                        return;
                    }

                    // Step 2: Modify the KeyCustomFrameRate value in the JSON
                    var gameQualitySetting = JObject.Parse(gameQualitySettingJson);
                    gameQualitySetting["KeyCustomFrameRate"] = 120;
                    string updatedGameQualitySettingJson = gameQualitySetting.ToString();

                    Console.WriteLine("\nUpdated GameQualitySetting JSON:");
                    Console.WriteLine(updatedGameQualitySettingJson);

                    // Step 3: Update the modified JSON back into the database
                    string updateQuery = "UPDATE LocalStorage SET value = @value WHERE key = 'GameQualitySetting';";

                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@value", updatedGameQualitySettingJson);
                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        Console.WriteLine($"\n{rowsAffected} row(s) đã được cập nhật.\nĐã bật 120 FPS!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            Console.WriteLine("\nẤn nút bất kì để thoát...");
            Console.ReadKey();
        }
    }
}