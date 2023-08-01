using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using ZXClient.model;

namespace ZXClient.dao

{
    public class db_EmployeeLoginDao
    {
        public static bool existsByCard(string card)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                DataTable data = ExecuteDataTable("SELECT * FROM EmployeeLogin where card='" + card+"'", null);
                return data.Rows.Count > 0;
            }
        }

        /// <summary>
        /// 获取最后一次登录的账号
        /// </summary>
        /// <returns></returns>
        internal static string[] getLastAutoLogin()
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                DataTable data = ExecuteDataTable("SELECT card,pwd,isMember FROM EmployeeLogin order by lastLogin desc limit 1", null);// where isMember=1
                if (data.Rows.Count > 0)
                {
                    DataRow dr = data.Rows[0];
                    return new string[] {dr[0].ToString(), dr[1].ToString(), dr[2].ToString() };
                }
            }
            return null;
        }

        internal static string getByCard(string card)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                SQLiteParameter[] parameters = new SQLiteParameter[] {
                    new SQLiteParameter() { ParameterName="@card", Value = card }
                };
                DataTable data = ExecuteDataTable("SELECT value FROM EmployeeLogin where card=@card limit 1", parameters);
                if (data.Rows.Count > 0)
                {
                    DataRow dr = data.Rows[0];
                    return dr[0].ToString();
                }
            }
            return String.Empty;
        }

        public static int addIfNoExist(string card, string pwd, int isMember)
        {
            if (existsByCard(card))
                return 0;
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO EmployeeLogin(id, card, pwd, isMember, lastLogin)" +
                    " VALUES (@id, @card, @pwd, @isMember, @lastLogin)";
                cmd.Parameters.AddWithValue("@id", null);
                cmd.Parameters.AddWithValue("@card", card);
                cmd.Parameters.AddWithValue("@pwd", pwd);
                cmd.Parameters.AddWithValue("@isMember", isMember);
                cmd.Parameters.AddWithValue("@lastLogin", DateTime.Now);
                return cmd.ExecuteNonQuery();
            }
        }

        public static int update(string card, string pwd, int isMember)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "update EmployeeLogin set pwd=@pwd, isMember=@isMember, lastLogin=@lastLogin where card=@card";
                cmd.Parameters.AddWithValue("@pwd", pwd);
                cmd.Parameters.AddWithValue("@isMember", isMember);
                cmd.Parameters.AddWithValue("@lastLogin", DateTime.Now);
                cmd.Parameters.AddWithValue("@card", card);
                return cmd.ExecuteNonQuery();
            }
        }
        
        public static DataTable ExecuteDataTable(string sql, SQLiteParameter[] parameters)
        {
            DataTable data = new DataTable();

            SQLiteCommand command = new SQLiteCommand(sql, MainData.conn);
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(data);

            return data;
        }

        internal static void logout(string card)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "update EmployeeLogin set isMember=0 where card=@card";
                cmd.Parameters.AddWithValue("@card", card);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
