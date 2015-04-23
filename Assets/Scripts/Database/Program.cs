
using System;
using System.Data;
using Mono.Data.SqliteClient;

namespace SQLiteSamples
{
    class Program
    {
        // Holds our connection with the database
        SqliteConnection _Database;
        Program _Program;

        static void Main(string[] args)
        {
            //Program _Program = new Program();
        }

        public Program()
        {
            createNewDatabase();
            connectToDatabase();
            createTable();
            fillTable();
            printBodySoup();
        }

        // Creates an empty database file
        void createNewDatabase()
        {
            //SqliteConnection.CreateFile("MyDatabase.sqlite");
        }

        // Creates a connection with our database file.
        void connectToDatabase()
        {
            _Database = new SqliteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            _Database.Open();
        }

        // Creates a table named 'bodysoup' with 2 colums
        void createTable()
        {
            //string sql = "create table bodysoup_main (id int(11), GestureCompleted varchar(255), kinnectjoint int)";
            //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
        }

        // Inserts some values in the bodysoup table.
        // As you can see, there is quite some duplicate code here, we'll solve this in part two.
        void fillTable()
        {
            //string sql = "insert into bodysoup_main (GestureCompleted, kinnectjoint) values ('test1', 3000)";
            //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
            //sql = "insert into bodysoup_main (GestureCompleted, kinnectjoint) values ('test2', 6000)";
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
            //sql = "insert into bodysoup_main (GestureCompleted, kinnectjoint) values ('test3', 9001)";
            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();
        }

        void printBodySoup()
        {
            //string sql = "select * from bodysoup_main order by id desc";
            //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            //SQLiteDataReader reader = command.ExecuteReader();
            //while (reader.Read())
                //Console.WriteLine("GestureCompleted: " + reader["GestureCompleted"] + "\t  KinnectJoint: " + reader["kinnectjoint"]);
            //Console.ReadLine();
        }
    }
}
