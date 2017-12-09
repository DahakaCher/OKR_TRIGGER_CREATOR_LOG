using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace OKR_trigger_creator_log
{
    public partial class Form1 : Form
    {

        Connection_DB_Data conn_data;
        

        SqlConnection DB_connection;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Connect_DB()
        {
            try
            {
                DB_connection = new SqlConnection(conn_data.Get_Connection_String());
                DB_connection.Open();
             //   MessageBox.Show("Sucsessfully connected");
            }
            catch
            {
               
            }
        }

      
        public void Get_Table_Names()
        {

            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_TYPE != 'VIEW'";

            try
            {
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                
                for (int i = 0; i < dt.Rows.Count; i++) comboBox1.Items.Add(dt.Rows[i].ItemArray[0].ToString());
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }

        }

        public void Select_Table(int index)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT * FROM " + comboBox1.Items[index].ToString(); 

            try
            {
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                dataGridView1.DataSource = dt;
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }

        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_DB_connect temp_form = new Form_DB_connect();
            try
            {
                conn_data = temp_form.Run();
                Connect_DB();
                Get_Table_Names();
                // MessageBox.Show(conn_data.Get_Connection_String());
            }
            catch
            {
                MessageBox.Show("Error 1: Connection error");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Select_Table(comboBox1.SelectedIndex);
        }
    }
}
