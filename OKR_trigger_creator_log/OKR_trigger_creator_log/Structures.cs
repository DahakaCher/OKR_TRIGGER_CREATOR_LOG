using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Connection_DB_Data()
        {
            DB_name = "";
            Server_name = "";
            User_ID = "";
            Password = "";
            Create_Connection_String();

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
}
