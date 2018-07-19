using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using ZXClient.model;

namespace ZXClient.dao

{
    public class db_ResUpdateDao
    {
        public static void add(string mac, int ver, int states)
        {
            //这里演示如何插入一条新的记录，如果要插入多条记录需要使用“事务”机制来提高效率  
            //你可以调用ExecuteNonQuery函数来简化下面的代码  
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO ResUpdate(pkid, mac, ver,states,cdate)" +
                    " VALUES  (@pkid, @mac, @ver, @states, @cdate)";
                cmd.Parameters.AddWithValue("@pkid", null);
                cmd.Parameters.AddWithValue("@ver", ver);
                cmd.Parameters.AddWithValue("@mac", mac);
                cmd.Parameters.AddWithValue("@states", states);
                cmd.Parameters.AddWithValue("@cdate", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        public static int update(string mac, int ver, int states)
        {
            //这里演示如何插入一条新的记录，如果要插入多条记录需要使用“事务”机制来提高效率  
            //你可以调用ExecuteNonQuery函数来简化下面的代码  
            using (SQLiteCommand cmd = (SQLiteCommand)MainData.conn.CreateCommand())
            {
                cmd.CommandText = "update ResUpdate set states=@states where mac=@mac and ver=@ver";
                cmd.Parameters.AddWithValue("@ver", ver);
                cmd.Parameters.AddWithValue("@mac", mac);
                cmd.Parameters.AddWithValue("@states", states);
                return cmd.ExecuteNonQuery();
            }
        }

        public static int? selectlast(String mac, int ver)
        {
            SQLiteParameter[] parameters = new SQLiteParameter[] {
                new SQLiteParameter() { ParameterName="ver", Value = ver },
                new SQLiteParameter() { ParameterName="mac", Value = mac }
            };
            DataTable data = ExecuteDataTable("SELECT states FROM ResUpdate where mac=@mac and ver=@ver order by pkid desc", parameters);
            if (data.Rows.Count > 0)
            {
                DataRow dr = data.Rows[0];
                return Convert.ToInt32(dr[0]);
            }
            return null;
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
