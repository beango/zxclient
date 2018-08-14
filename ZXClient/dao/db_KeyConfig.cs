using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using ZXClient.model;
using ZXClient.util;

namespace ZXClient.dao
{
    public class db_KeyConfig
    {
        public static bool exists(string usercard, string actionName)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                DataTable data = ExecuteDataTable("SELECT * FROM KeyConfig where actionName='"+actionName+"' and usercard='"+ usercard + "'", null);
                return data.Rows.Count > 0;
            }
        }

        internal static Dictionary<string, string> getKeyConfig(string usercard)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                DataTable data = null;
                if (String.IsNullOrEmpty(usercard))
                    data = ExecuteDataTable("SELECT actionName, key FROM KeyConfig where usercard is null", null);
                else
                    data = ExecuteDataTable("SELECT actionName, key FROM KeyConfig where usercard='" + usercard + "'", null);
                    
                if (data.Rows.Count > 0)
                {
                    Dictionary<string, string> keydic = new Dictionary<string, string>();
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        if(!keydic.ContainsKey(data.Rows[i][0].ToString()))
                            keydic.Add(data.Rows[i][0].ToString(), data.Rows[i][1].ToString());
                    }
                    return keydic;
                }
            }
            return null;
        }

        public static int addIfNoExist(string usercard, string actionName, String key)
        {
            if (exists(usercard,actionName))
                return 0;
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO KeyConfig(id, usercard, actionName, key)" +
                    " VALUES  (@id, @usercard, @actionName, @key)";
                cmd.Parameters.AddWithValue("@id", null);
                cmd.Parameters.AddWithValue("@usercard", usercard);
                cmd.Parameters.AddWithValue("@actionName", actionName);
                cmd.Parameters.AddWithValue("@key", key);
                int r = cmd.ExecuteNonQuery();
                LogHelper.WriteInfo(typeof(db_KeyConfig), "添加快捷键:" + key + "," + usercard + "," + actionName + ",结果:" + r);
                return r;
            }
        }

        public static int update(string key, string usercard, string actionName)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "update KeyConfig set key=@key where usercard=@usercard and actionName=@actionName";
                cmd.Parameters.AddWithValue("@key", key);
                cmd.Parameters.AddWithValue("@usercard", usercard);
                cmd.Parameters.AddWithValue("@actionName", actionName);
                int r = cmd.ExecuteNonQuery();
                LogHelper.WriteInfo(typeof(db_KeyConfig),"更新快捷键:" + key + "," + usercard + "," + actionName + ",结果:" + r);
                return r;
            }
        }

        public static int updateByKey(string key, string value)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "update KeyConfig set " + key+"=@value";
                cmd.Parameters.AddWithValue("@value", value);
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

        
    }
}
