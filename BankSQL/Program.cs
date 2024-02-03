using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace BankSQL
{
   
    class Bank
    {
        static String connString = @"Data Source=bank.db";
        










    }
    class Program
    {
        static String connString = @"Data Source=bank.db";
        static bool isAuth=false;
        

        public  static void Main(string[] args)
        {
            Bank app = new Bank();

            using (var connection= new SqliteConnection(connString))
            {
              
              connection.Open();
              var cmd= connection.CreateCommand();
              cmd.CommandText =
               @"CREATE TABLE IF NOT EXISTS users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username VARCHAR,
                        Password VARCHAR,
                        Amount  INTEGER
                        )";

               

                cmd.ExecuteNonQuery();
              connection.Close();

            }

            Home();




        }



        public static void Home()
        {
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("--BANK APP with SQL--");
                Console.WriteLine("---------------------");

                Console.WriteLine("1.Register Account");
                Console.WriteLine("2.Login");
                /*  Console.WriteLine("3.Update todo");
                  Console.WriteLine("4.Delete todo");
                  Console.WriteLine("5.Clear Table");*/

                Console.WriteLine("x.Close App\n");

                Console.Write("Make your Choice: ");
                String choice = Console.ReadLine();
                Console.WriteLine("\n\n");

                switch (choice)
                {
                    case "1":
                        //getAllTasks();
                        Register();
                        break;
                    case "2":
                        Login();
                        while (isAuth == true)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("--Welcome Back--");
                            Console.WriteLine("---------------------");
                            Console.WriteLine("1.Deposit money");
                            Console.WriteLine("2.Withdraw money");
                            Console.WriteLine("x. Logout");

                            Console.Write("Make your choice: ");
                            string option=Console.ReadLine();
                            switch (option)
                            {
                                case "1":
                                    deposit();
                                    break;
                                case "2":
                                    withdraw();
                                    break;

                                case "x":
                                    isAuth = false;
                                    break;
                            }
                        }
                        break;
                    case "3":
                        //Update();
                        break;
                    case "4":
                        //Delete();
                        break;
                    case "5":
                        //ClearTable();
                        break;
                    case "x":
                        closeApp = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error.Please make a choice between 1-3 Or 'x' to close the app");
                        break;
                }
            }
        }

        public static void Register()
        {
            Console.WriteLine("\n---Create an Account---");
            String username = Uname();
            String password = Pwd();
            int depositAmt = Amt("Amount you want to deposit to activate account: $");

            using (var connection = new SqliteConnection(connString))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = $"INSERT INTO users(Username,Password,Amount) VALUES('{username}','{password}','{depositAmt}')";



                cmd.ExecuteNonQuery();
                connection.Close();
            }


            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"User Account '{username}' is created");

        }
        internal static string Uname()
        {
           
            Console.Write("Your Username: ");
            string username = Console.ReadLine();



            return username;
        }

        internal static string Pwd()
        {

            Console.Write("Your Password: ");
            string password = Console.ReadLine();


            return password;
        }

        internal static int Amt(string Message)
        {

            Console.Write($"{Message}");
            int amount = int.Parse(Console.ReadLine());


            return amount;
        }


        public static void Login()
        {
            Console.WriteLine("--Login Your Account --");
            String username = Uname();
            String password = Pwd();

            using(var connection= new SqliteConnection(connString))
            {
                connection.Open();
                var cmd= connection.CreateCommand();
                //cmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM users WHERE Username = '{username}' ";
                cmd.CommandText = $"SELECT count(*) FROM users WHERE Username='{username}' AND Password = '{password}'  ";
                //WHERE Password = '{password}'
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The user ' {username}'  doesnt not exists");
                    cmd.ExecuteNonQuery();
                    connection.Close();
                   
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Welcome back {username}");
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    isAuth = true;
                }
               
               

            }
      
        }
        public static void deposit()
        {
            int deposit = Amt("Enter amount you wish to Deposit: $");
            String username = Uname();
            String password = Pwd();

            using (var connection = new SqliteConnection(connString))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT Amount FROM users WHERE Username=@username AND Password=@password";
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                int currentAmt = 0;
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int Amount = reader.GetInt32(0);
                        currentAmt = Amount;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Hello {username}, your old amount is: {currentAmt:C}\n");
                    }
                    else
                    {
                        Console.WriteLine("User not found or no data in the 'amount' column.");
                        return; // Exit the method if the user is not found
                    }
                }

                // Execute the update statement to deposit the amount
                cmd.CommandText = "UPDATE users SET Amount = Amount + @deposit WHERE Username=@username AND Password=@password";
                cmd.Parameters.AddWithValue("@deposit", deposit);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Success. Amount ${deposit} deposited successfully. Current balance: ${deposit + currentAmt}\n");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to update the amount.");
                }

                connection.Close();
            }
        }



        public static void withdraw()
        {
            int withdrawalAmt =Amt("Enter amount you wish to Withdraw: $");
            String username = Uname();
            String password = Pwd();
            using (var connection = new SqliteConnection(connString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"SELECT Amount FROM users  WHERE Username='{username}' AND Password = '{password}'  ";
                int currentAmt = 0;
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                       
                        int Amount = reader.GetInt32(0);
                        currentAmt = Amount;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"User's current amount: {currentAmt:C}\n");
                    }
                    else
                    {
                        Console.WriteLine("User not found or no data in the 'amount' column.");
                    }
                }
                if (withdrawalAmt > currentAmt  )
                {
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error.Your account balance is not sufficient.Current balance:{currentAmt}\n");
                }
                else
                {
                    cmd.CommandText = $" UPDATE users  SET Amount = Amount - {withdrawalAmt} WHERE Username='{username}' AND Password = '{password}'  ";
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Success.Amount ${withdrawalAmt} withdrawn succesfully.Current balance: ${withdrawalAmt-currentAmt}\n");
                }
            

            }
        }
























    }
}
