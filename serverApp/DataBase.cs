using Npgsql;
using serverApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using lib;

namespace serverApp
{
    internal class DataBase
    {
        string connectionStr;
        string dbname;

        public DataBase(string connStr_, string dbname_) {
            connectionStr = connStr_;
            dbname = dbname_;

            //подключение к базе данных
        }
   
        public bool chkDBExists(){
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionStr))
            {
                using (NpgsqlCommand command = new NpgsqlCommand
                    ($"SELECT DATNAME FROM pg_catalog.pg_database WHERE DATNAME = '{dbname}'", conn))
                {
                    try
                    {
                        conn.Open();
                        var i = command.ExecuteScalar();
                        conn.Close();
                        if (i.ToString().Equals(dbname)) //always 'true' (if it exists) or 'null' (if it doesn't)
                            return true;
                        else return false;
                    }
                    catch (Exception e) { return false; }
                }
            }
        }

        public bool SearchAccount(string login, string password) {
            NpgsqlConnection conn = new NpgsqlConnection(connectionStr);
            using NpgsqlCommand command = new NpgsqlCommand
                    ($"SELECT password FROM public.\"Accounts\" WHERE login = '{login}'", conn);
            conn.Open();
            using var reader = command.ExecuteReader();
            
            reader.ReadAsync();
            if (reader.HasRows)
            {
                var pass = reader.GetString(0);
                conn.Close();
                if (password == pass)
                    return true;
            }
            conn.Close();
            return false;
            
        }

        public Client autorization(Client client)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionStr);
            NpgsqlCommand command = new NpgsqlCommand
                    ($"SELECT ac.id_dep, dep.department FROM public.\"Accounts\" ac, public.\"Department\" dep WHERE ac.login = '{client.userName}' AND ac.id_dep = dep.id_dep", conn);
            conn.Open();
            using var reader = command.ExecuteReader();

            reader.ReadAsync();
            if (reader.HasRows)
            {
                client.id_dep = reader.GetInt32(0);
                client.dep = reader.GetString(1);
            }
            else
            {
                client.id_dep = 0;
            }
            conn.Close();
            client.isAuthorized = true;

            return client;
        }

        public List<Person> EndingEDS(Client client)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionStr);
            // использование view "endingEDSRequest"
            NpgsqlCommand command = new NpgsqlCommand
                ($"SELECT * From endingEDSRequest", conn);
            conn.Open();
            using var reader = command.ExecuteReader();
            List<Person> res = new List<Person>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    //Создание объекта класса с полями запроса
                    Person pers = new Person(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    WorkPlace wp = new WorkPlace(reader.GetString(4), reader.GetString(5));
                    pers.AddWorkPlace(wp);
                    pers.AddEDS(new EDS(reader.GetDateTime(6), reader.GetString(7)));

                    //Сотрудники по отделению клиента
                    if ((client.id_dep == 0) || (wp.department == client.dep)) {
                        res.Add(pers);                
                    }
                }
            }
            return res;
        }

        public void /*Person*/ GetPersonByID(int id)
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionStr);
            conn.Open();

            // персональные данные сотрудника
            NpgsqlCommand command = new NpgsqlCommand
                    ($"SELECT * FROM public.\"Staff\" s WHERE s.id_pers = {id}");
            
            var per = command.ExecuteScalar();

            // места работы сотрудника
            command = new NpgsqlCommand
                    ($"SELECT * FROM public.\"WorkPlace\" wp WHERE wp.id_pers = {id}");
            var wp = command.ExecuteScalar();

            // ЭЦП сотрудника
            command = new NpgsqlCommand
                    ($"SELECT eds.id_pers, eds.applicationnumber, eds.certnumber, eds.dateofissue, eds.dateofend, org.orgname, rs.status FROM public.\"EDS\" eds, public.\"Organization\" org, public.\"RegistrationStat\" rs WHERE eds.id_regstat = rs.id_regstat and eds.id_org = org.id_org and eds.id_pers = {id}");
            var eds = command.ExecuteScalar();

            // документы сотрудника
            command = new NpgsqlCommand
                    ($"SELECT * FROM public.\"Document\" doc WHERE doc.id_pers = {id}");
            var docs = command.ExecuteScalar();

            // действия по каждой эцп

            conn.Close();

            //Person pers = new Person();

          //  return pers;
        }


        public bool insertRow(Person person) {

            if (chkDBExists())
            {

                try
                {                
                    NpgsqlConnection conn = new NpgsqlConnection(connectionStr);
                    NpgsqlCommand command = new NpgsqlCommand
                        ($"SELECT DATNAME FROM pg_catalog.pg_database WHERE DATNAME = '{dbname}'", conn);
                    conn.Open();

                }
                catch (Exception e) { return false; }
                return true;
            }
            else return false;
        }
    }
}
