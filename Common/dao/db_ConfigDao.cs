using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Common.model;

namespace Common.dao

{
    public class db_ConfigDao
    {
        public static bool exists()
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                DataTable data = ExecuteDataTable("SELECT * FROM Config", null);
                return data.Rows.Count > 0;
            }
        }

        internal static object[] getConfig()
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                DataTable data = ExecuteDataTable("SELECT ServerAddr,ServerIP, ServerPort,DeviceIP,ConnType," +
                    "FtpIP,FtpPort,FtpUserName,FtpPwd,isNoLogin FROM Config limit 1", null);
                if (data.Rows.Count > 0)
                {
                    DataRow dr = data.Rows[0];
                    
                    return dr.ItemArray;
                }
            }
            return null;
        }

        public static int addIfNoExist(string ServerAddr, string ServerIP, string ServerPort, String DeviceIP
            ,string ConnType)
        {
            if (exists())
                return 0;
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Config(id, ServerAddr,ServerIP, ServerPort, DeviceIP, ConnType, isNoLogin)" +
                    " VALUES  (@id, @ServerAddr, @ServerIP, @ServerPort, @DeviceIP, @ConnType, @isNoLogin)";
                cmd.Parameters.AddWithValue("@id", null);
                cmd.Parameters.AddWithValue("@ServerAddr", ServerAddr);
                cmd.Parameters.AddWithValue("@ServerIP", ServerIP);
                cmd.Parameters.AddWithValue("@ServerPort", ServerPort);
                cmd.Parameters.AddWithValue("@DeviceIP", DeviceIP);
                cmd.Parameters.AddWithValue("@ConnType", ConnType);
                cmd.Parameters.AddWithValue("@isNoLogin", false);
                return cmd.ExecuteNonQuery();
            }
        }

        public static int update(string ServerAddr, string ServerIP, string ServerPort)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "update Config set ServerAddr=@ServerAddr,ServerIP=@ServerIP,ServerPort=@ServerPort ";
                cmd.Parameters.AddWithValue("@ServerAddr", ServerAddr);
                cmd.Parameters.AddWithValue("@ServerIP", ServerIP);
                cmd.Parameters.AddWithValue("@ServerPort", ServerPort);
                return cmd.ExecuteNonQuery();
            }
        }

        public static int updateByKey(string key, object value)
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "update Config set "+key+"=@value";
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

        internal static void UpdateSchema()
        {
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "select sql from sqlite_master where type='table' and tbl_name='Config'";
                String sql = (String)cmd.ExecuteScalar();
                if (!sql.Contains("isNoLogin"))
                {
                    cmd.CommandText = "ALTER TABLE [Config] Add [isNoLogin] BOOLEAN(1)";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
