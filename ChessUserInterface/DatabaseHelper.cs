using System.Data.SQLite;
using System.IO;

namespace ChessUserInterface
{
    public static class DatabaseHelper
    {
        private const string DatabaseName = "users.db";

        public static void InitializeDatabase()
        {
            if (!File.Exists(DatabaseName))
            {
                SQLiteConnection.CreateFile(DatabaseName);

                using (var connection = new SQLiteConnection($"Data Source={DatabaseName};Version=3;"))
                {
                    connection.Open();
                    string sql = @"
                    CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE,
                        Password TEXT,
                        Email TEXT,
                        RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                        LastLogin DATETIME,
                        Wins INTEGER DEFAULT 0,
                        Losses INTEGER DEFAULT 0,
                        Draws INTEGER DEFAULT 0,
                        TotalGames INTEGER DEFAULT 0,
                        Rating INTEGER DEFAULT 1200,
                    )";
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                UpdateDatabaseSchemaIfNeeded();
            }
        }

        public static List<MainMenu.User> GetLeaders()
        {
            var leaders = new List<MainMenu.User>();

            using (var connection = new SQLiteConnection($"Data Source={DatabaseName};Version=3;"))
            {
                connection.Open();
                string sql = "SELECT Username, Wins FROM Users ORDER BY Wins DESC LIMIT 10"; // Получаем топ-10 игроков по количеству побед
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new MainMenu.User
                        {
                            Name = reader["Username"].ToString(),
                            Wins = Convert.ToInt32(reader["Wins"])
                        };
                        leaders.Add(user);
                    }
                }
            }

            return leaders;
        }

        private static void UpdateDatabaseSchemaIfNeeded()
        {
            using (var connection = new SQLiteConnection($"Data Source={DatabaseName};Version=3;"))
            {
                connection.Open();
                string sql = "PRAGMA table_info(Users)";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    var existingColumns = new HashSet<string>();
                    while (reader.Read())
                    {
                        existingColumns.Add(reader["name"].ToString());
                    }

                    var columnsToAdd = new List<string>();
                    if (!existingColumns.Contains("Email"))
                    {
                        columnsToAdd.Add("ALTER TABLE Users ADD COLUMN Email TEXT");
                    }
                    if (!existingColumns.Contains("RegistrationDate"))
                    {
                        columnsToAdd.Add("ALTER TABLE Users ADD COLUMN RegistrationDate DATETIME CURRENT_TIMESTAMP");
                    }
                    if (!existingColumns.Contains("LastLogin"))
                    {
                        columnsToAdd.Add("ALTER TABLE Users ADD COLUMN LastLogin DATETIME");
                    }
                    if (!existingColumns.Contains("Wins"))
                    {
                        columnsToAdd.Add("ALTER TABLE Users ADD COLUMN Wins DEFAULT 0");
                    }
                    if (!existingColumns.Contains("Losses"))
                    {
                        columnsToAdd.Add("ALTER TABLE Users ADD COLUMN Losses DEFAULT 0");
                    }
                    if (!existingColumns.Contains("Draws"))
                    {
                        columnsToAdd.Add("ALTER TABLE Users ADD COLUMN Draws DEFAULT 0");
                    }
                    if (!existingColumns.Contains("TotalGames"))
                    {
                        columnsToAdd.Add("ALTER TABLE Users ADD COLUMN TotalGames DEFAULT 0");
                    }
                    if (!existingColumns.Contains("Rating"))
                    {
                        columnsToAdd.Add("ALTER TABLE Users ADD COLUMN Rating DEFAULT 1200");
                    }

                    foreach (var columnSql in columnsToAdd)
                    {
                        using (var alterCommand = new SQLiteCommand(columnSql, connection))
                        {
                            alterCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public static void AddUser(string username, string password, string email)
        {
            using (var connection = new SQLiteConnection($"Data Source={DatabaseName};Version=3;"))
            {
                connection.Open();
                string sql = "INSERT INTO Users (Username, Password, Email) VALUES (@Username, @Password, @Email)";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Email", email);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool ValidateUser(string username, string password)
        {
            using (var connection = new SQLiteConnection($"Data Source={DatabaseName};Version=3;"))
            {
                connection.Open();
                string sql = "SELECT COUNT(1) FROM Users WHERE Username=@Username AND Password=@Password";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    return (long)command.ExecuteScalar() == 1;
                }
            }
        }

        public static void IncrementWins(string username)
        {
            UpdateUserStatistics(username, "Wins");
        }

        public static void IncrementLosses(string username)
        {
            UpdateUserStatistics(username, "Losses");
        }

        public static void IncrementDraws(string username)
        {
            UpdateUserStatistics(username, "Draws");
        }

        private static void UpdateUserStatistics(string username, string column)
        {
            using (var connection = new SQLiteConnection($"Data Source={DatabaseName};Version=3;"))
            {
                connection.Open();
                string sql = $"UPDATE Users SET {column} = {column} + 1, TotalGames = TotalGames + 1 WHERE Username = @Username";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static int GetWins(string username)
        {
            return GetUserStatistic(username, "Wins");
        }

        public static int GetLosses(string username)
        {
            return GetUserStatistic(username, "Losses");
        }

        public static int GetDraws(string username)
        {
            return GetUserStatistic(username, "Draws");
        }

        private static int GetUserStatistic(string username, string column)
        {
            using (var connection = new SQLiteConnection($"Data Source={DatabaseName};Version=3;"))
            {
                connection.Open();
                string sql = $"SELECT {column} FROM Users WHERE Username = @Username";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }
}
