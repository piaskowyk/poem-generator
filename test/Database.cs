using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace test
{
    public class Database
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public Database(){
            Initialize();
        }

        private void Initialize(){
            this.server = "localhost";
            this.database = "poem";
            this.uid = "root";
            this.password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";CharSet=utf8;";
            this.connection = new MySqlConnection(connectionString);
        }

        public void transactionOpen(){
            connection.Open();
        }

        public void transactionClose(){
            connection.Close();
        }

        public MySqlConnection getConnection(){
            return this.connection;
        }

        public DataSet query(string cmd){
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd, this.connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet, "data");
            return dataSet;
        }

        public int insert(string cmd){
            DataSet ds = query(cmd + "; SELECT LAST_INSERT_ID() as id;");
            DataTable dt = ds.Tables["data"];
            DataRow dr = dt.Rows[0];
			return Convert.ToInt32(dr["id"]);
        }
    }
}
