using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace pilot
{
    class Program
    {
        static void Main(string[] args)
        {
            string db_name = "pilot";
            bool isConnected = false;
            SqlConnectionStringBuilder conn_str = new SqlConnectionStringBuilder();
            conn_str.ConnectionString = @"Server=localhost\SQLEXPRESS;Database=pilot;Trusted_Connection=True;";

            SqlConnection sqlConnection = new SqlConnection(conn_str.ConnectionString);
            String str = @"CREATE DATABASE " + db_name;

            SqlCommand cmd = new SqlCommand(str, sqlConnection);
            try
            {
                sqlConnection.Open();
                if (sqlConnection.Database == "pilot")
                    isConnected = true;

                if (!isConnected)
                {
                    if (cmd.Connection.ConnectionString == conn_str.ConnectionString) // *commenet this condition scope to run again
                    {
                        if (sqlConnection.Database != "pilot")
                            cmd.ExecuteNonQuery();
                        Console.WriteLine("database is created successfully\n");
                        sqlConnection.ChangeDatabase("pilot");
                        create_tables(cmd, sqlConnection);
                    } // *
                }
                Console.WriteLine($"Connected to DataBase: {sqlConnection.Database} \n");

                static void create_tables(SqlCommand cmd, SqlConnection con)
                {
                    cmd.CommandText = @"CREATE TABLE Parts(catalogNum VARCHAR(255) NOT NULL PRIMARY KEY, partName VARCHAR(255) NOT NULL, date DATETIME NOT NULL, userCode INT NOT NULL, langCode INT NOT NULL)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO Parts(catalogNum, partName, date, userCode, langCode) VALUES('001', 'Galaxy',  { ts '2023-02-24 10:02:20' }" +
                        ", 222, 972)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE Machines(machineName VARCHAR(255) NOT NULL PRIMARY KEY, date DATETIME NOT NULL, userCode INT NOT NULL, langCode INT NOT NULL)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO Machines(machineName, date, userCode, langCode) VALUES('Machine 8201', { ts '2023-02-24 10:02:20' }, 222, 972)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE Work_Orders(orderNum VARCHAR(255) NOT NULL PRIMARY KEY, catalogNum VARCHAR(255) NOT NULL FOREIGN KEY REFERENCES Parts(catalogNum), machineName VARCHAR(255) NOT NULL FOREIGN KEY REFERENCES Machines(machineName), quantity INT NOT NULL, date DATETIME NOT NULL, userCode INT NOT NULL, langCode INT NOT NULL)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "INSERT INTO Work_Orders(orderNum, catalogNum, machineName, quantity, date, userCode, langCode) VALUES('1', '001', 'Machine 8201', 100, { ts '2023-02-24 10:02:20' }, 222, 972)";
                    cmd.ExecuteNonQuery();
                }
                sqlConnection.Close();

                Model model = new Model(DateTime.Now, 333, 999);
                Part part = new Part("002", "S26", DateTime.Now, 221, 972);
                Machine machine = new Machine("Machine 3203", DateTime.Now, 222, 972);
                Work_Order workOrder = new Work_Order("2", "002", "Machine 3202", 100, DateTime.Now, 222, 972);

                string action_type = "0";
                Console.WriteLine("List of actions:\n" +
                        "1. Add new part\n" +
                        "2. Get parts list\n" +
                        "3. Add new machine\n" +
                        "4. Get machines list\n" +
                        "5. Add new work order\n" +
                        "6. Get work orders list\n");

                while (true)
                {
                    if (action_type == "8")
                        break;
                    Console.WriteLine("What would you like to do? (Enter 1-6, or 8 to escape)");
                    action_type = Console.ReadLine();

                    switch (action_type)
                    {
                        case "1":
                            Console.WriteLine("Please enter the details in the same order:\n" +
                                "Catalog Number, Part Name");
                            string part_details = Console.ReadLine().ToLower().Trim();
                            if (part_details == "")
                                continue;
                            var part_properties = part_details.Split(",");
                            Part new_part = new Part(part_properties[0], part_properties[1], model._date, model._userCode, model._langCode);
                            part.addPart(new_part, sqlConnection);
                            break;

                        case "2":
                            part.printParts(part, sqlConnection);
                            break;

                        case "3":
                            Console.WriteLine("Please enter the new Machine Name: ");
                            string machine_details = Console.ReadLine().ToLower().Trim();
                            if (machine_details == "")
                                continue;
                            Machine new_machine = new Machine(machine_details, model._date, model._userCode, model._langCode);
                            machine.addMachine(new_machine, sqlConnection);
                            break;

                        case "4":
                            machine.printMachines(machine, sqlConnection);
                            break;

                        case "5":
                            Console.WriteLine("Please enter the details in the same order (separated by a comma, without spaces):\n" +
                                "Order Number, Catalog Number, Machine Name, Quantity (nubers only) ");
                            string order_details = Console.ReadLine().ToLower().Trim();
                            if (order_details == "")
                                continue;
                            var order_properties = order_details.Split(",");
                            string orderNum = order_properties[0];
                            string catalogNum = order_properties[1];
                            string machineName = order_properties[2];
                            int quantity = Convert.ToInt32(order_properties[3]);
                            if (quantity.GetType().Name.ToString() != "Int32")
                                Console.WriteLine("Please enter numbers only for quantity. (press 5 to try again)");
                            Work_Order new_order = new Work_Order(orderNum, catalogNum, machineName, quantity, model._date, model._userCode, model._langCode);
                            bool valid = workOrder.isValid(new_order, part, machine, sqlConnection);
                            if (valid == true)
                                workOrder.addWorkOrder(new_order, sqlConnection);
                            break;

                        case "6":
                            workOrder.printWorkOrders(sqlConnection);
                            break;

                        default:
                            action_type = "8";
                            sqlConnection.Close();
                            Console.WriteLine("Good Bye");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            sqlConnection.Close();
        }
    }
}