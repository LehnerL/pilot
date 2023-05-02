using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace pilot
{
    class Work_Order : Model
    {
        string _orderNum { get; set; }
        string _catalogNum { get; set; }
        string _machineName { get; set; }
        int _quantity { get; set; }


        public Work_Order(string orderNum, string catalogNum, string machineName, int quantity, DateTime date, int userCode, int langCode) : base(date, userCode, langCode)
        {
            _orderNum = orderNum;
            _catalogNum = catalogNum;
            _machineName = machineName;
            _quantity = quantity;
            base.getInfo();
        }

        public Work_Order(DataRow dataRow) : base(dataRow)
        {
            _date = (DateTime)dataRow["date"];
            _userCode = (int)dataRow["userCode"];
            _langCode = (int)dataRow["langCode"];
        }

        public void printWorkOrders(SqlConnection conn_str)
        {
            try
            {
                string query = "SELECT * FROM Work_Orders";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn_str.ConnectionString);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset, "work_orders");
                Console.WriteLine("Work Orders List: \n");
                foreach (DataRow row in dataset.Tables["work_orders"].Rows)
                {
                    Console.WriteLine($"Order Numer: {row["orderNum"]}, Catalog Number: {row["catalogNum"]}, Machine Name: {row["machineName"]}," +
                        $" Quantity: {row["quantity"]}, Date: {row["date"]}, User Code: {row["userCode"]}, Language Code: {row["langCode"]} \n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public bool isValid(Work_Order work_order, Part part, Machine machine, SqlConnection conn_str)
        {
            bool valid = false;
            if (work_order._catalogNum == "" || work_order._machineName == "")
                Console.WriteLine("Catalog Number or Machine Name does not exist.");
            try
            {
                string query = "SELECT * FROM Work_Orders";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn_str.ConnectionString);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset, "work_orders");

                foreach (DataRow row in dataset.Tables["work_orders"].Rows)
                {
                    if (work_order._orderNum == (string)(row.ItemArray[0]))
                    {
                        Console.WriteLine("Order Numer already exist.\n");
                        return valid;
                    }
                }

                DataSet machine_dataset = machine.getMachines(conn_str);
                foreach (DataRow row in machine_dataset.Tables["machines"].Rows)
                {
                    if (work_order._machineName != (string)(row.ItemArray[0]))
                        valid = false;
                    else
                        valid = true;
                }

                DataSet part_dataset = part.getParts(conn_str);
                foreach (DataRow row in part_dataset.Tables["parts"].Rows)
                {
                    if (work_order._catalogNum != (string)(row.ItemArray[0]))
                        valid = false;
                    else
                        valid = true;
                }
               //Console.WriteLine(valid.ToString());
                if (valid == false)
                    Console.WriteLine("Catalog Number or Machine Name does not exist.\n");
                return valid;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return valid;
            }
        }

        public void addWorkOrder(Work_Order work_order, SqlConnection conn_str)
        {
            try
            {
                string query = "SELECT * FROM Work_Orders";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn_str.ConnectionString);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset, "work_orders");

                DataRow new_row = dataset.Tables["work_orders"].NewRow();
                new_row["orderNum"] = work_order._orderNum;
                new_row["catalogNum"] = work_order._catalogNum;
                new_row["machineName"] = work_order._machineName;
                new_row["quantity"] = work_order._quantity;
                new_row["date"] = work_order._date;
                new_row["userCode"] = work_order._userCode;
                new_row["langCode"] = work_order._langCode;
                dataset.Tables["work_orders"].Rows.Add(new_row);

                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.Update(dataset.Tables["work_orders"]);
                Console.WriteLine("work order added successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}