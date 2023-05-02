using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace pilot
{
    public class Part : Model
    {
        string _catalogNum { get; set; }
        string _partName { get; set; }


        public Part(string catalogNum, string partName, DateTime date, int userCode, int langCode) : base(date, userCode, langCode)
        {
            _catalogNum = catalogNum;
            _partName = partName;
            base.getInfo();
        }

        public Part(DataRow dataRow) : base(dataRow)
        {
            _date = (DateTime)dataRow["date"];
            _userCode = (int)dataRow["userCode"];
            _langCode = (int)dataRow["langCode"];
        }

        public DataSet getParts(SqlConnection conn_str)
        {
            DataSet dataset = new DataSet();
            try
            {
                string query = "SELECT * FROM Parts";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn_str.ConnectionString);
                adapter.Fill(dataset, "parts");
                return dataset;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return dataset;
            }
        }

        public void printParts(Part part, SqlConnection conn_str)
        {
            try
            {
                DataSet dataset = new DataSet();
                dataset = part.getParts(conn_str);
                Console.WriteLine("Parts List: \n");
                foreach (DataRow row in dataset.Tables["parts"].Rows)
                {
                    Console.WriteLine($"Catalog Number: {row["catalogNum"]}, Part Name: {row["partName"]}, Date: {row["date"]}," +
                        $" User Code: {row["userCode"]}, Language Code: {row["langCode"]} \n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void addPart(Part part, SqlConnection conn_str)
        {
            try
            {
                string query = "SELECT * FROM Parts";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn_str.ConnectionString);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset, "parts");
                foreach (DataRow row in dataset.Tables["parts"].Rows)
                {
                    if (part._catalogNum == (string)(row.ItemArray[0]))
                    {
                        Console.WriteLine($"Part Catalog Number already exist. \n" +
                            $" Catalog Number: {row["catalogNum"]}, Part Name: {row["partName"]}, Date: {row["date"]}," +
                        $" User Code: {row["userCode"]}, Language Code: {row["langCode"]} \n");
                        return;
                    }
                }
                DataRow new_row = dataset.Tables["parts"].NewRow();
                new_row["catalogNum"] = part._catalogNum;
                new_row["partName"] = part._partName;
                new_row["date"] = part._date;
                new_row["userCode"] = part._userCode;
                new_row["langCode"] = part._langCode;
                dataset.Tables["parts"].Rows.Add(new_row);

                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.Update(dataset.Tables["parts"]);
                Console.WriteLine("part added successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}