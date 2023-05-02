using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace pilot
{
    public class Machine : Model
    {
        string _machineName { get; set; }


        public Machine(string machineName, DateTime date, int userCode, int langCode) : base(date, userCode, langCode)
        {
            _machineName = machineName;
            //_date = date.ToLocalTime();
            //_userCode = userCode;
            //_langCode = langCode;
            base.getInfo();
        }

        public Machine(DataRow dataRow) : base(dataRow)
        {
            _date = (DateTime)dataRow["date"];
            _userCode = (int)dataRow["userCode"];
            _langCode = (int)dataRow["langCode"];
        }

        public DataSet getMachines(SqlConnection conn_str)
        {
            DataSet dataset = new DataSet();
            try
            {
                string query = "SELECT * FROM Machines";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn_str.ConnectionString);
                adapter.Fill(dataset, "machines");
                return dataset;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return dataset;
            }
        }

        public void printMachines(Machine machine, SqlConnection conn_str)
        {
            try
            {
                DataSet dataset = new DataSet();
                dataset = machine.getMachines(conn_str);
                Console.WriteLine("Machines List: \n");
                foreach (DataRow row in dataset.Tables["machines"].Rows)
                {
                    Console.WriteLine($"Machine: {row["machineName"]}, Date: {row["date"]}," +
                        $" User Code: {row["userCode"]}, Language Code: {row["langCode"]} \n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void addMachine(Machine machine, SqlConnection conn_str)
        {
            try
            {
                string query = "SELECT * FROM Machines";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn_str.ConnectionString);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset, "machines");

                foreach (DataRow row in dataset.Tables["machines"].Rows)
                {
                    if (machine._machineName == (string)(row.ItemArray[0]))
                    {
                        Console.WriteLine($"Machine already exist. \n " +
                            $"{row["machineName"]}, {row["date"]}, {row["userCode"]}, {row["langCode"]} \n");
                        return;
                    }
                }
                DataRow new_row = dataset.Tables["machines"].NewRow();
                new_row["machineName"] = machine._machineName;
                new_row["date"] = machine._date;
                new_row["userCode"] = machine._userCode;
                new_row["langCode"] = machine._langCode;
                dataset.Tables["machines"].Rows.Add(new_row);

                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.Update(dataset.Tables["machines"]);
                Console.WriteLine("machine added successfully.");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}