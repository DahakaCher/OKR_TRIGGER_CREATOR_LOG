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
    public partial class Form_Update : Form
    {
        public Form_Update()
        {
            InitializeComponent();
        }

        Connection_DB_Data data;
        String table_name;
        int ID;
        SqlConnection DB_connection;
        List<TextBox> L_textBox;
        List<Label> L_names, L_types;
        

        public void Connect_DB()
        {
            try
            {
                DB_connection = new SqlConnection(data.Get_Connection_String());
                DB_connection.Open();
                //   MessageBox.Show("Sucsessfully connected");
            }
            catch
            {

            }
        }

        public void func_insert()
        {
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "INSERT INTO " + table_name + " (";
            for (int i = 1; i < L_names.Count; i++)  if (i == 1) cmd.CommandText += " " + L_names[i].Text + ""; else cmd.CommandText += " ," + L_names[i].Text + "";
            cmd.CommandText += ")  Values (";
            for (int i = 1; i < L_textBox.Count; i++) if (i == 1) cmd.CommandText += " '" + L_textBox[i].Text + "'"; else cmd.CommandText += " ,'" + L_textBox[i].Text + "'";
            cmd.CommandText += ")";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }
        }

        private void button1_Click_ins(object sender, EventArgs e)
        {
            func_insert();
            this.Close();
        }

        private void button1_Click_upd(object sender, EventArgs e)
        {
            func_update();
            this.Close();
        }

        public bool Run(Connection_DB_Data DBcondata)
        {
            data = DBcondata;
            this.ShowDialog();
            return true;
        }

        public bool Run_Insert(Connection_DB_Data DBcondata, String tbl_n)
        {
            this.button1.Click += new System.EventHandler(this.button1_Click_ins);
            data = DBcondata;
            table_name = tbl_n;
            ID = -1;
            Connect_DB();
            Form_Creating();
            this.ShowDialog();
            return true;
        }

        public bool Run_Update(Connection_DB_Data DBcondata, String tbl_n, int id)
        {
            this.button1.Click += new System.EventHandler(this.button1_Click_upd);
            data = DBcondata;
            table_name = tbl_n;
            ID = id;
            Connect_DB();
            Form_Creating();
            Fill_textBox();
            this.ShowDialog();
            return true;
        }

        public void func_update()
        {
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "UPDATE " + table_name + " SET ";
            for (int i = 1; i < L_names.Count; i++) if (i == 1) cmd.CommandText += L_names[i].Text + " = '" + L_textBox[i].Text+ "'"; else cmd.CommandText += " ," + L_names[i].Text + " = " + L_textBox[i].Text;
            cmd.CommandText += " WHERE " + L_names[0].Text + " = " + ID.ToString();
           
           

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }
        }

        public void Fill_textBox()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT * FROM "+table_name+" WHERE "+ L_names[0].Text + " = " + ID.ToString();

            try
            {
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);   

                for (int i = 0; i < dt.Rows[0].ItemArray.Length; i++) L_textBox[i].Text = dt.Rows[0].ItemArray[i].ToString();             

                L_textBox[0].ReadOnly = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }
        }

        public void Form_Creating()
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = DB_connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT column_name, data_type FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '"+ table_name+"'";

         try
            {
                cmd.ExecuteNonQuery();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            //215,20 texbox size
            L_textBox = new List<TextBox>();
            L_types = new List<Label>();
            L_names = new List<Label>();


            for (int i = 0; i<dt.Rows.Count; i++)
                {
                    Label ln = new Label();
                    ln.Text = dt.Rows[i].ItemArray[0].ToString();
                    ln.Location = new System.Drawing.Point(10, 10+30*i);
                ln.Size = new System.Drawing.Size(100, 20);
                L_names.Add(ln);

                    TextBox tb = new TextBox();
                    tb.Text = "";
                    tb.Location = new System.Drawing.Point(120, 10 + 30 * i);
                    tb.Size = new System.Drawing.Size(220, 20);
                    L_textBox.Add(tb);

                    Label lt = new Label();
                    lt.Text = dt.Rows[i].ItemArray[1].ToString();
                    lt.Location = new System.Drawing.Point(350, 10 + 30 * i);
                lt.Size = new System.Drawing.Size(60, 20);
                L_types.Add(lt);

                    this.panel1.Controls.Add(L_names[i]);
                    this.panel1.Controls.Add(L_textBox[i]);
                    this.panel1.Controls.Add(L_types[i]);

                }

                L_textBox[0].ReadOnly = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка запиту (" + ex.Message + ") : " + cmd.CommandText);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_Update_Load(object sender, EventArgs e)
        {
           
        }
    }
}
