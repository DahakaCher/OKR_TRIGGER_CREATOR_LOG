using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace OKR_trigger_creator_log
{

    class Structures
    {
    }

    public class Connection_DB_Data
    {

        public String DB_name;
        public String Server_name;
        public String User_ID;
        public String Password;
        private String Connection_String;
        public bool Reset;

        public Connection_DB_Data()
        {
            DB_name = "";
            Server_name = "";
            User_ID = "";
            Password = "";
            Create_Connection_String();
            Reset = false;

        }

        public Connection_DB_Data(String A, String B, String C, String D)
        {
            DB_name = B;
            Server_name = A;
            User_ID = C;
            Password = D;
            Create_Connection_String();
        }

        public void Create_Connection_String()
        {
            this.Connection_String = "Data Source=" + Server_name + "; Initial Catalog =" + DB_name + "; User ID =" + User_ID + "; Password =" + Password;
        }

        public String Get_Connection_String()
        {           
            return this.Connection_String;
        }
    }

    public class Triggering
    {
        private Connection_DB_Data conn_data;
        SqlConnection DB_connection;
        public String LOG_Table_name;
        String INSERT_tr_name;
        String UPDATE_tr_name;
        String DELETE_tr_name;
        List<String> names;

        public Triggering(Connection_DB_Data data)
        {
            this.conn_data = data;

            LOG_Table_name = "LOGS_TABLE";

            INSERT_tr_name = "TRIGGER_LOG_INSERT";
            UPDATE_tr_name = "TRIGGER_LOG_UPDATE";
            DELETE_tr_name = "TRIGGER_LOG_DELETE";

            Connect_DB();
            Get_Table_Names();
            Create_LOG_Table();
            Create_Triggers();
        }

        public void Connect_DB()
        {
            try
            {
                DB_connection = new SqlConnection(conn_data.Get_Connection_String());
                DB_connection.Open();            
            }
            catch
            {

            }
        }

        private void ResetTriggers()
        {

            Get_Table_Names();

            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "IF OBJECT_ID('"+LOG_Table_name+"', 'U') IS NOT NULL DROP TABLE " + LOG_Table_name;

            try
            {             
                cmd.ExecuteNonQuery();

                for (int i=0; i< names.Count; i++)
                {
                    cmd.CommandText = "IF OBJECT_ID ('" + DELETE_tr_name + "_" + names[i] + "', 'TR') IS NOT NULL DROP TRIGGER " + DELETE_tr_name + "_" + names[i];
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "IF OBJECT_ID ('" + UPDATE_tr_name + "_" + names[i] + "', 'TR') IS NOT NULL DROP TRIGGER " + UPDATE_tr_name + "_" + names[i];
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "IF OBJECT_ID ('" + INSERT_tr_name + "_" + names[i] + "', 'TR') IS NOT NULL DROP TRIGGER " + INSERT_tr_name + "_" + names[i];
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }
        }

        private void Create_LOG_Table()
        {

            if (conn_data.Reset) ResetTriggers();

            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_TYPE != 'VIEW' AND TABLE_NAME = '" + LOG_Table_name+"'";

            try
            {
                if (cmd.ExecuteScalar() != null)  return;

                cmd.CommandText = "CREATE TABLE [dbo].["+ LOG_Table_name +"]("
                                    + "[ID_LOG][int] IDENTITY(1, 1) NOT NULL,"
                                    + "[Action] [nchar](6) NULL,"
                                    + "[Table_name] [nchar](100) NULL,"
                                    + "[DateTime] [date] NULL,"
                                    + "[Username] [nchar](100) NULL,"
                                    + "[Information] [nchar](800) NULL, "
                                    + "CONSTRAINT[PK_"+ LOG_Table_name +"] PRIMARY KEY CLUSTERED ("
                                     + "[ID_LOG] ASC"
                                        + ")WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]"
                                       + ") ON[PRIMARY]";

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }
        }

        public void Get_Table_Names()
        {
            names = new List<String>();

            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_TYPE != 'VIEW'";

            try
            {
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++) if (dt.Rows[i].ItemArray[0].ToString() != "sysdiagrams" && dt.Rows[i].ItemArray[0].ToString() != LOG_Table_name) names.Add(dt.Rows[i].ItemArray[0].ToString());
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }

        }

        public List<String> Get_Column_Names(String TableName )
        {
           List<String> cols = new List<String>();

            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT COLUMN_NAME  FROM information_schema.columns WHERE TABLE_NAME = '"+TableName+"'";

            try
            {
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++) cols.Add(dt.Rows[i].ItemArray[0].ToString());
                return cols;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
                return null;
            }

        }

        public String create_info_text()
        {
            String tmp = "";


            return tmp;
        }


        private void Create_Triggers()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            SqlCommand cmd1 = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd1.CommandType = CommandType.Text;

         //   cmd.CommandText = "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_TYPE != 'VIEW'";

            try
            {
            

            

                for (int i = 0; i < names.Count; i++)
                {
                    
                        // INSERT
                        cmd1.CommandText = "select [name] from sysobjects where xtype = 'TR' and parent_obj = (select id from sysobjects where name = '" + names[i] + "' ) and[name] = '" + INSERT_tr_name + "_" + names[i] + "'";
                        if (cmd1.ExecuteScalar() == null)
                        {
                            cmd1.CommandText = "CREATE TRIGGER [dbo].[" + INSERT_tr_name+"_"+names[i] + "] "
                                                + "ON [dbo].[" + names[i] + "] AFTER INSERT AS "
                                                + "INSERT INTO [dbo].[" + LOG_Table_name + "] ([Action], [Table_name], [DateTime], [Username], [Information]) "
                                                + "VALUES ('INSERT', '" + names[i] + "', GETDATE(), SUSER_NAME(),'Inserted ID = ' +LTRIM(STR((SELECT ID_" + names[i] + " FROM inserted))))";
                                                
                            cmd1.ExecuteNonQuery();
                        }

                        // UPDATE
                        cmd1.CommandText = "select [name] from sysobjects where xtype = 'TR' and parent_obj = (select id from sysobjects where name = '" + names[i] + "' ) and[name] = '" + UPDATE_tr_name + "_" + names[i] + "'";
                        if (cmd1.ExecuteScalar() == null)
                        {
                            cmd1.CommandText = "CREATE TRIGGER [dbo].[" + UPDATE_tr_name+"_" + names[i] + "] "
                                                + "ON [dbo].[" + names[i] + "] AFTER UPDATE AS "
                                                + "INSERT INTO [dbo].[" + LOG_Table_name + "] ([Action], [Table_name], [DateTime], [Username], [Information]) "
                                                + "VALUES ('UPDATE', '" + names[i] + "', GETDATE(), SUSER_NAME(), 'Update ID = ' +LTRIM(STR((SELECT ID_" + names[i] + " FROM inserted))))  ";
                                               
                            cmd1.ExecuteNonQuery();
                        }

                        // DELETE
                        cmd1.CommandText = "select [name] from sysobjects where xtype = 'TR' and parent_obj = (select id from sysobjects where name = '" + names[i] + "' ) and[name] = '" + DELETE_tr_name + "_" + names[i] + "'";
                        if (cmd1.ExecuteScalar() == null)
                        {
                            cmd1.CommandText = "CREATE TRIGGER [dbo].[" + DELETE_tr_name+"_" + names[i] + "] "
                                                + "ON [dbo].[" + names[i] + "] AFTER DELETE AS "
                                                + "INSERT INTO [dbo].["+LOG_Table_name+"] ([Action], [Table_name], [DateTime], [Username], [Information]) "
                                                + "VALUES ('DELETE', '" + names[i] + "', GETDATE(), SUSER_NAME(), 'Deleted ID = ' +LTRIM(STR((SELECT ID_"+names[i]+" FROM deleted)))) ";
                                               
                            cmd1.ExecuteNonQuery();
                        }

                    

                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText + " // " +cmd1.CommandText);
            }
        }
    }
}
