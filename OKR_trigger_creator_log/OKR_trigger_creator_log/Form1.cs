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
        Triggering trig;

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
                trig = new Triggering(conn_data);

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
                
                for (int i = 0; i < dt.Rows.Count; i++) if (dt.Rows[i].ItemArray[0].ToString() != "sysdiagrams" ) comboBox1.Items.Add(dt.Rows[i].ItemArray[0].ToString());
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
                toolStripStatusLabel1.Text = "Connected to : " + conn_data.Server_name + " Data Base : " + conn_data.DB_name + " as " + conn_data.User_ID; 
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

        private void button2_Click(object sender, EventArgs e)
        {
            Form_Update tmp_form = new Form_Update();
            tmp_form.Run_Insert(conn_data, comboBox1.Items[comboBox1.SelectedIndex].ToString());
            Select_Table(comboBox1.SelectedIndex);
        }

      public void func_delete()
        {
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "DELETE FROM " + comboBox1.Items[comboBox1.SelectedIndex].ToString() + " WHERE " + dataGridView1.Columns[0].HeaderText + " = " + dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            try
            {

                DialogResult res = MessageBox.Show("DELETE selected raw ?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes) cmd.ExecuteNonQuery();
                // cmd.ExecuteNonQuery();   

            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form_Update tmp_form = new Form_Update();
            tmp_form.Run_Update(conn_data, comboBox1.Items[comboBox1.SelectedIndex].ToString(), Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value));
            Select_Table(comboBox1.SelectedIndex);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            func_delete();
            Select_Table(comboBox1.SelectedIndex);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void not_log_table_view(bool status)
        {

            button2.Enabled = status;
            button3.Enabled = status;
            button4.Enabled = status;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Select_Table(comboBox1.SelectedIndex);

            if (comboBox1.Items[comboBox1.SelectedIndex].ToString() == trig.LOG_Table_name) not_log_table_view(false); else not_log_table_view(true);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Done by Dahaka (c)");
        }
    }
}
