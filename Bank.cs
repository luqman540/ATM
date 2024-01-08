using System;
using System.Data.SqlClient;
using System.Security;
using System.Threading;

namespace ATM_Code
{
    class Bank
    {
        private static string connectionString = @"Data Source=TECHFWD-LUQMAN\SQLEXPRESS;Initial Catalog=ATM;Integrated Security=True";
        private static int AccountID = 0;
        private static ConsoleColor green = ConsoleColor.Green;
        private static ConsoleColor red = ConsoleColor.Red;
        private static ConsoleColor blue = ConsoleColor.Blue;
        private static string Message = "";
        private static bool Successful = true;
        static void Main(string[] args)
        {

            WelcomeScreen(10);
            Thread.Sleep(2000);
            bool login = AtmPinScreen();
            if (login)
            {
                DisplayMenu();
            }
            ThankYouScreen(Message);


        }
        private static void WelcomeScreen(int y)
        {

            Console.Clear();
            Console.ForegroundColor = blue;
            Console.SetCursorPosition(30, y++);
            MakeStar(43);
            Console.SetCursorPosition(30, y++);
            Console.WriteLine("|\t\tWelcome To TechForward ATM\t|");
            Console.SetCursorPosition(30, y++);
            Console.WriteLine("|\t\t--------------------------\t|");
            Console.SetCursorPosition(30, y++);
            Console.WriteLine("|\t\tDevloped By: Luqman Waheed\t|");
            Console.SetCursorPosition(30, y++);
            MakeStar(43);
            Console.ResetColor();
        }

        private static bool AtmPinScreen()
        {
            WelcomeScreen(2);
            Console.SetCursorPosition(25, 10);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                Console.Write("Enter your 4-digit Pin Number = ");
                string enteredPin = FourDigitPin();

                if (int.TryParse(enteredPin, out int pin) || enteredPin != null)
                {
                    string query = "SELECT * FROM Users WHERE Pin = @Pin";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Pin", pin);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                AccountID = Convert.ToInt32(reader["AccountID"]);
                                con.Close();
                                return true;
                            }
                            else
                            {
                                Console.SetCursorPosition(25, 13);
                                Message = "Invalid PIN....";
                                Successful = false;
                            }
                        }
                    }
                }
                else
                {
                    Console.SetCursorPosition(25, 13);
                    Message = "Invalid PIN....";
                    Successful = false;
                }
                con.Close();
                return false;
            }

        }

        private static void DisplayMenu()
        {
            WelcomeScreen(2);
            Console.SetCursorPosition(25, 9);
            Console.WriteLine("\t\t  Main Menu");
            Console.SetCursorPosition(25, 10);
            Console.WriteLine("\t-------------------------------");
            Console.SetCursorPosition(25, 11);
            Console.WriteLine("\t For Balance Inquery (Press B)");
            Console.SetCursorPosition(25, 12);
            Console.WriteLine("\t For Cash Withdrawal (Press C)");
            Console.SetCursorPosition(25, 13);
            Console.WriteLine("\t For Fund Transfer (Press F)");
            Console.SetCursorPosition(25, 14);
            Console.WriteLine("\t For Pin Change (Press P)");
            Console.SetCursorPosition(25, 15);
            Console.WriteLine("\t To Exit (Press X)");
            Console.SetCursorPosition(25, 16);
            Console.WriteLine("\t-------------------------------");
            Console.SetCursorPosition(25, 17);
            Console.Write("Select Any Option from Above Menu: ");
            ConsoleKeyInfo input = Console.ReadKey();
            switch (Convert.ToChar(input.KeyChar))
            {
                case 'B':
                case 'b':
                    Inquery();
                    break;
                case 'C':
                case 'c':
                    WithDraw();
                    break;
                case 'F':
                case 'f':
                    TransferFundScreen();
                    break;
                case 'P':
                case 'p':
                    PinChangeScreen();
                    break;
                case 'X':
                case 'x':
                    break;
                default:
                    DisplayMenu();
                    break;
            }

        }

        static void Inquery()
        {
            WelcomeScreen(2);
            Console.SetCursorPosition(25, 10);
            decimal balance = CurrentBalance();
            Console.WriteLine("Your Current Balance is " + balance);
            Thread.Sleep(2000);
            DisplayMenu();
        }

        static void WithDraw()
        {
            bool wantRecipt = false;
            WelcomeScreen(2);
            Console.SetCursorPosition(25, 10);
            Console.WriteLine("Please Enter Amount Multiples of Rs 500 or Rs 1000");
            MakeBox(40, 12, 15);
            Console.SetCursorPosition(45, 13);
            var inputAmount = Console.ReadLine();
            if (decimal.TryParse(inputAmount, out decimal withdrawAmount))
            {
                if (withdrawAmount > 0 && withdrawAmount % 500 == 0)
                {
                    bool isValid = false;
                    do
                    {
                        WelcomeScreen(2);
                        Console.SetCursorPosition(25, 10);
                        Console.WriteLine("Do you want a recipt of this Transaction?");
                        Console.SetCursorPosition(25, 11);
                        Console.WriteLine("Recipt Charges: Rs 2.5");
                        Console.SetCursorPosition(25, 12);
                        Console.Write("Press Y for Yes and N for No : ");
                        ConsoleKeyInfo inputRecipt = Console.ReadKey();
                        if (Convert.ToChar(inputRecipt.KeyChar) == 'Y' || Convert.ToChar(inputRecipt.KeyChar) == 'y')
                        {
                            wantRecipt = true;
                            isValid = true;
                        }
                        else if (Convert.ToChar(inputRecipt.KeyChar) == 'N' || Convert.ToChar(inputRecipt.KeyChar) == 'n')
                        {
                            isValid = true;
                        }
                    }
                    while (isValid != true);
                    WithDrawMoney(withdrawAmount, wantRecipt);
                }
                else
                {
                    Successful = false;
                    Message = "Invalid Amount Entered....";
                }
            }
            else
            {
                Successful = false;
                Message = "Invalid Amount Entered....";
            }

        }
        static void WithDrawMoney(decimal amount, bool recipt)
        {
            if (recipt)
            {
                amount = amount + 2.5m;
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                if (AccountID > 0)
                {
                    con.Open();

                    string selectQuery = "SELECT Balance FROM Users WHERE AccountID = @AccountID";
                    string updateQuery = "UPDATE Users SET Balance = @NewBalance WHERE AccountID = @AccountID";

                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, con))
                    {
                        selectCmd.Parameters.AddWithValue("@AccountID", AccountID);
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal currentBalance = Convert.ToDecimal(reader["Balance"]);
                                decimal remainingBalance = currentBalance - amount;
                                if (remainingBalance >= 0)
                                {
                                    reader.Close();
                                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                                    {
                                        updateCmd.Parameters.AddWithValue("@NewBalance", remainingBalance);
                                        updateCmd.Parameters.AddWithValue("@AccountID", AccountID);

                                        updateCmd.ExecuteNonQuery();
                                        Message = "Transaction Successful";
                                    }
                                }
                                else
                                {
                                    Message = "Insufficent Balance.";
                                    Successful = false;

                                }
                            }
                            else
                            {
                                Message = "Server Issue, Please Try Again Later....";
                                Successful = false;
                            }
                        }
                    }

                }
                con.Close();
            }
        }
        static void TransferFundScreen()
        {
            WelcomeScreen(2);
            Console.SetCursorPosition(25, 10);
            Console.Write("Please Enter 8-digit Account Number : ");
            var transferAccountId = Console.ReadLine();
            if (int.TryParse(transferAccountId, out int accountId) && accountId != AccountID)
            {
                WelcomeScreen(2);
                Console.SetCursorPosition(27, 10);
                Console.WriteLine("(Amount must be Multiples of Rs 500 or Rs 1000)");
                Console.SetCursorPosition(24, 12);
                Console.Write("Please Enter Amount You want to Transfer : ");
                var amount = Console.ReadLine();
                if (decimal.TryParse(amount, out decimal transferAmount) && transferAmount % 500 == 0)
                {
                    TransferFund(accountId, transferAmount);
                }
                else
                {
                    Message = "Invalid Amount Entered....";
                    Successful = false;
                }
            }
            else
            {
                Message = "Invalid Account Number....";
                Successful = false;
            }
        }

        static void TransferFund(int transferAccountId, decimal transferAmount)
        {
            decimal transferAccountBalance = -3, userBalance = -3;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string selectQuery = "Select * from Users where AccountID = @TranferAccount";
                using (SqlCommand selectCmd = new SqlCommand(selectQuery, con))
                {
                    selectCmd.Parameters.AddWithValue("@TranferAccount", transferAccountId);
                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            transferAccountBalance = Convert.ToDecimal(reader["Balance"]);
                        }
                    }
                }
                using (SqlCommand selectCmd2 = new SqlCommand(selectQuery, con))
                {
                    selectCmd2.Parameters.AddWithValue("@TranferAccount", AccountID);
                    using (SqlDataReader reader = selectCmd2.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userBalance = Convert.ToDecimal(reader["Balance"]);

                        }
                    }
                }
                con.Close();
            }
            if (transferAccountBalance >= 0 && userBalance >= 0)
            {
                string updateQuery = "UPDATE Users SET Balance = @Balance WHERE AccountID = @AccountId";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    if (AccountID > 0)
                    {
                        decimal remainingBalance = userBalance - transferAmount;
                        if (remainingBalance >= 0)
                        {
                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                            {
                                updateCmd.Parameters.AddWithValue("@Balance", remainingBalance);
                                updateCmd.Parameters.AddWithValue("@AccountId", AccountID);

                                updateCmd.ExecuteNonQuery();
                            }
                            using (SqlCommand updateCmd2 = new SqlCommand(updateQuery, con))
                            {
                                updateCmd2.Parameters.AddWithValue("@Balance", transferAccountBalance + transferAmount);
                                updateCmd2.Parameters.AddWithValue("@AccountId", transferAccountId);

                                updateCmd2.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            Successful = false;
                            Message = "Insufficent Balance....";
                        }
                    }
                    con.Close();
                }
            }
            else
            {
                Successful = false;
                Message = "Invalid Account Number....";
            }
            if (Successful)
            {
                Message = "Fund Transfer Successfully....";
            }

            //using (SqlConnection con = new SqlConnection(connectionString))
            //{
            //    if (AccountID > 0)
            //    {
            //        con.Open();
            //        string selectQuery = "Select * from Users where AccountID in (@AccountId, @TranferAccount)";
            //        using (SqlCommand selectCmd = new SqlCommand(selectQuery, con))
            //        {
            //            selectCmd.Parameters.AddWithValue("@AccountId", AccountID);
            //            selectCmd.Parameters.AddWithValue("@TranferAccount", transferAccountId);

            //            using (SqlDataReader reader = selectCmd.ExecuteReader())
            //            {
            //                while (reader.Read())
            //                {
            //                    decimal currentBalance = Convert.ToDecimal(reader["Balance"]);
            //                    decimal remainingBalance = 0.0m;
            //                    int queryAccountId = Convert.ToInt32(reader["AccountID"]);
            //                    if (queryAccountId == transferAccountId)
            //                    {
            //                        currentBalance = 
            //                        remainingBalance = currentBalance + transferAmount;
            //                    }
            //                    else if(queryAccountId == AccountID && currentBalance >= transferAmount)
            //                    {
            //                        remainingBalance = currentBalance - transferAmount;
            //                    }
            //                    else
            //                    {
            //                        Successful = false;
            //                        if(currentBalance < transferAmount)
            //                            Message = "Insufficent Balance....";
            //                        else
            //                            Message = "Server Issue, Please Try Again Later....";
            //                        return;
            //                    }
            //                    using (SqlConnection con2 = new SqlConnection(connectionString))
            //                    {
            //                        con2.Open();

            //                        string updateQuery = "UPDATE Users SET Balance = @Balance WHERE AccountID = @AccountId";

            //                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, con2))
            //                        {
            //                            updateCmd.Parameters.AddWithValue("@Balance", remainingBalance);
            //                            updateCmd.Parameters.AddWithValue("@AccountId", queryAccountId);

            //                            int rowsAffected = updateCmd.ExecuteNonQuery();

            //                            if (rowsAffected < 1)
            //                            {
            //                                Successful = false;
            //                                Message = "Server Issue, Please Try Again Later....";
            //                                return;
            //                            }
            //                        }
            //                        con2.Close();
            //                    }
            //                }

            //            }
            //        }
            //    }
            //    Message = "Fund Transfer Successfully....";
            //}
        }
        static void PinChangeScreen()
        {
            string strPin = "";
            string strRePin = "";
            string errorMess = "";
            int Pin = 0;
            bool isError = false;
            do
            {
                WelcomeScreen(2);
                if (isError == true)
                {
                    Console.ForegroundColor = red;
                    Console.SetCursorPosition(35, 8);
                    Console.WriteLine(errorMess);
                    Console.ResetColor();
                }
                Console.SetCursorPosition(25, 10);
                Console.Write("Please Enter Your New Pin : ");
                strPin = FourDigitPin();
                Console.SetCursorPosition(25, 11);
                Console.Write("Please Enter Your Pin Again : ");
                strRePin = FourDigitPin();
                if (int.TryParse(strPin, out Pin) == false)
                {
                    errorMess = "Enter a Valid 4-digit Pin";
                    isError = true;
                }
                else if (strPin != strRePin)
                {
                    errorMess = "Pin does not Match";
                    isError = true;
                }
                else
                {
                    isError = false;
                }
            } while (isError);
            if (Pin > 0)
            {
                ChangePin(Pin);
            }
            //Console.ForegroundColor = green;
            //Console.SetCursorPosition(27, 14);
            //Console.WriteLine("Pin Changed Successfully....");
            //Console.ResetColor();
            //Thread.Sleep(2000);
            //DisplayMenu();
        }
        static string FourDigitPin()
        {
            SecureString pass = new SecureString();
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
                var charPressed = keyInfo.KeyChar;
                if (!char.IsControl(charPressed))
                {
                    pass.AppendChar(charPressed);
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass.RemoveAt(pass.Length - 1);
                    Console.Write("\b \b");
                }
            } while (pass.Length < 4);
            return new System.Net.NetworkCredential(string.Empty, pass).Password;
        }
        static void ChangePin(int pin)
        {
            string query = "Update Users SET Pin = @Pin where AccountID = @AccountId";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                if (AccountID > 0)
                {
                    using (SqlCommand updateCmd = new SqlCommand(query, con))
                    {
                        updateCmd.Parameters.AddWithValue("@Pin", pin);
                        updateCmd.Parameters.AddWithValue("@AccountId", AccountID);

                        updateCmd.ExecuteNonQuery();
                        Message = "Pin Changed Successfully....";
                    }
                }
                con.Close();
            }
        }
        static void ThankYouScreen(string message)
        {
            WelcomeScreen(2);
            if (Successful)
                Console.ForegroundColor = green;
            else
                Console.ForegroundColor = red;
            Console.SetCursorPosition(42, 10);
            Console.WriteLine(message);
            Console.ResetColor();
            Console.SetCursorPosition(38, 12);
            Console.WriteLine("Please Take Your Card....");
            ThankYouMessage(15);
            Console.ReadLine();
        }

        static void ThankYouMessage(int y)
        {
            Console.ForegroundColor = blue;
            Console.SetCursorPosition(34, y++);
            MakeStar(35);
            Console.SetCursorPosition(34, y++);
            Console.WriteLine("|  Thank You For using This ATM   |");
            Console.SetCursorPosition(34, y++);
            MakeStar(35);
            Console.ResetColor();
        }
        static decimal CurrentBalance()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                if (AccountID > 0)
                {
                    string query = "SELECT * FROM Users WHERE AccountID = @AccountId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AccountId", AccountID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal balance = Convert.ToDecimal(reader["Balance"]);
                                con.Close();
                                return balance;
                            }
                        }
                    }
                }
                con.Close();
                return 0;
            }
        }

        static void MakeStar(int no)
        {
            for (int i = 0; i < no; i++)
            {
                Console.Write("*");
            }
        }
        static void MakeBox(int x, int y, int stars)
        {
            Console.SetCursorPosition(x, y++);
            MakeStar(stars);
            Console.SetCursorPosition(x, y);
            Console.WriteLine("|");
            Console.SetCursorPosition(x + (stars - 1), y++);
            Console.WriteLine("|");
            Console.SetCursorPosition(x, y++);
            MakeStar(stars);
        }
    }
}
