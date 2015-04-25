using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;

public class Program {
	
	public Program(SqliteConnection dbconn) {
        	
		dbconn.Open();

		//creating table
		string sql = "create table bodysoup_main (id int(11), GestureCompleted varchar(255), kinnectjoint int)";
		SqliteCommand command = new SqliteCommand(sql, dbconn);
		command.ExecuteNonQuery();

		//inserting data
		string sql2 = "insert into bodysoup_main (GestureCompleted, kinnectjoint) values ('test1', 3000)";
		SqliteCommand command2 = new SqliteCommand(sql2, dbconn);
		command2.ExecuteNonQuery();

		//reading info
		try {
			SqliteCommand cmd = new SqliteCommand("SELECT GestureCompleted FROM bodysoup_main", dbconn);
			SqliteDataReader reader = cmd.ExecuteReader();
				
			while (reader.Read()) {
				Debug.Log ("yes");
				Console.WriteLine("GestureCompleted: " + reader["GestureCompleted"] + "\t  KinnectJoint: " + reader["kinnectjoint"]);
				Console.ReadLine();
			}

			reader.Close();
			reader = null;
				
			cmd.Dispose();
			cmd = null;
			dbconn.Close();
			dbconn = null;

		} catch {
			Debug.Log ("nope");
			Console.WriteLine("fail");
			Console.ReadLine();
		}
	}
}
