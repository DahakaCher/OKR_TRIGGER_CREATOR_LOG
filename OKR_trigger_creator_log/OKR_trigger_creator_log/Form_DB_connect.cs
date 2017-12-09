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
    public partial class Form_DB_connect : Form
    {
        public Form_DB_connect()
        {
            InitializeComponent();
        }

        Connection_DB_Data data;
        SqlConnection DB_connection;


        public Connection_DB_Data Run()
        {
            
            this.ShowDialog();
            return data;

        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Read_Data()
        {
            data = new Connection_DB_Data(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
         
            /*  data.Server_name = textBox1.Text;
            data.DB_name = textBox2.Text;
            data.User_ID = textBox3.Text;
            data.Password = textBox4.Text;*/

            data.Create_Connection_String();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Read_Data();
            try
            {

                DB_connection = new SqlConnection(data.Get_Connection_String());
                DB_connection.Open();
                MessageBox.Show("Sucsessfully connected");
                this.Close();
            }
            catch
            {
                MessageBox.Show("Connection error");
            }
        }
    }
}
