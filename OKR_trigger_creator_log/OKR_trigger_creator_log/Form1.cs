using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OKR_trigger_creator_log
{
    public partial class Form1 : Form
    {

        Connection_DB_Data conn_data;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_DB_connect temp_form = new Form_DB_connect();
            try
            {
                conn_data = temp_form.Run();
                MessageBox.Show(conn_data.Get_Connection_String());
            }
            catch
            {
                MessageBox.Show("Error 1");
            }
        }
    }
}
