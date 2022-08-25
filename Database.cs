using System.Data.SQLite;

namespace Bot
{
    class Database
    {
        public SQLiteConnection DBConnection;

        public Database()
        {
            DBConnection = new SQLiteConnection("Data Source=database.sqlite3");

            if (!File.Exists("./database.sqlite3"))
            {
                SQLiteConnection.CreateFile("database.sqlite3");
                Console.WriteLine("Succefully created database");
            }
        }

        public void OpenConnection()
        {
            if (DBConnection.State != System.Data.ConnectionState.Open)
            {
                DBConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (DBConnection.State != System.Data.ConnectionState.Closed)
            {
                DBConnection.Clone();
            }
        }
    }
}