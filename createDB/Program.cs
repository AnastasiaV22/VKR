using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace createDB
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string connStr = "Server=localhost;Port=1602;User Id=postgres;Password=1234;";
            var m_conn = new NpgsqlConnection(connStr);
            var m_createdb_cmd = new NpgsqlCommand(@"
                CREATE DATABASE EDStestBD2
                WITH OWNER = postgres
                ENCODING = 'UTF8'
                CONNECTION LIMIT = -1;
                ", m_conn);

            m_conn.Open();
            m_createdb_cmd.ExecuteNonQuery();
            m_conn.Close();
        }
    }
}
