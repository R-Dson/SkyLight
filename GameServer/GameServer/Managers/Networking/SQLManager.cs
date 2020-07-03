﻿using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;

namespace Server.Managers
{
    class SQLManager
    {

        private static string connectionString; 
        private static MySqlConnection cn;

        public static void SetUpSQL(string ID, string cred)
        {
            connectionString = "Server=db4free.net;Port=3306;Connect Timeout=2147483;User Id=" + ID + ";password=" + cred + ";Database=skylighttemp;old guids=true;";
            try
            {
                cn = new MySqlConnection(connectionString);
                cn.Open();
                Debug.WriteLine("Connected to database.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to connect to database. " + e.Message);
            }
        }

        public static void CloseSQL()
        {
            cn.Close();
        }

        public static void UpdateToSQL(string username, string data)
        {

            var updateSQL = "UPDATE Users(accountInfo, accountData, accountInfoSecond) VALUES(@accountData) WHERE accountInfo = @usr";
            //"UPDATE Users SET accountInfoSecond = 'katt' WHERE accountInfo = '' "

            using (var cmd = new MySqlCommand(updateSQL, cn))
            {

                //cmd.Parameters.AddWithValue("@accountInfo", username);
                cmd.Parameters.AddWithValue("@accountData", data);
                cmd.Parameters.AddWithValue("@usr", username);
                cmd.Prepare();
                cmd.ExecuteNonQuery();

            }


        }

        public static void AddToSQL(string username, string password, string data)
        {
            //if username already in database
            if (CheckIfExistInSQL(username))
            {

                return;
            }

            var InserSQL = "INSERT INTO Users(accountInfo, accountData, accountInfoSecond) VALUES(@accountInfo, @accountData, @accountInfoSecond)";

            using (var cmd = new MySqlCommand(InserSQL, cn))
            {
                cmd.Parameters.AddWithValue("@accountInfo", username);
                cmd.Parameters.AddWithValue("@accountData", data);
                cmd.Parameters.AddWithValue("@accountInfoSecond", password);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                Console.WriteLine("Added \"" + username + " : " + data + "\" to Database");
            }


        }

        internal static string[] GetDataFromSQL(string username)
        {
            string[] returner = { "", "" };

            var sqlGet = "SELECT * FROM Users WHERE accountInfo = '@usr'";
            sqlGet = "SELECT * FROM Users WHERE accountInfo = '" + username + "'";

            using (var cmd = new MySqlCommand(sqlGet, cn))
            {
                //cmd.Parameters.AddWithValue("@usr", username);
                string s = cmd.CommandText;
                cmd.Prepare();

                using (MySqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        returner[0] = rd["accountData"].ToString();
                        returner[1] = rd["accountInfoSecond"].ToString();
                    }
                }
            }
            return returner;
        }

        public static bool CheckIfExistInSQL(string username)
        {
            //TODO: Proper sql request
            var sqlCheck = "SELECT COUNT(1) FROM Users WHERE accountInfo = '" + username + "'";

            using (var cmd = new MySqlCommand(sqlCheck, cn))
            {
                //cmd.Parameters.AddWithValue("@user", username);
                cmd.Prepare();
                var c = Convert.ToInt32(cmd.ExecuteScalar());
                if (c > 0)
                {
                    return true;
                }

            }

            return false;
        }

    }
}
