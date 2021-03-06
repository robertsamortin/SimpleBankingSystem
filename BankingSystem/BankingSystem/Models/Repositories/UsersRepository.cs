﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BankingSystem.Models.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        //Change connection to the database
        string connectionString = "Server =.\\SQLEXPRESS;Database=Banking;Integrated Security = true;";
        SqlTransaction sqlTrans = null;

        public void AddUser(Users usr)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    sqlTrans = con.BeginTransaction();
                    SqlCommand cmd = new SqlCommand("spUserInsert", con, sqlTrans);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@LoginName", usr.LoginName);
                    cmd.Parameters.AddWithValue("@AccountNumber", usr.AccountNumber);
                    cmd.Parameters.AddWithValue("@Password", usr.Password);
                    cmd.Parameters.AddWithValue("@Balance", usr.Balance);
                    cmd.Parameters.AddWithValue("@CreatedDate", usr.CreatedDate);

                    cmd.ExecuteNonQuery();
                    sqlTrans.Commit();
                }
                catch
                {
                    if (sqlTrans != null)
                    {
                        sqlTrans.Rollback();
                    }
                }
                finally
                {
                    con.Close();
                }
                
            }
        }

        public bool CheckLoginName(string LoginName)
        {
            bool result = false;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserCheckLoginName", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@LoginName", LoginName);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }


                con.Close();
            }
            return result;
        }

        public Users GetUserByID(string ID)
        {
            Users usr = new Users();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spUserGetByID", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID", ID);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        usr.ID = Convert.ToInt32(rdr["ID"]);
                        usr.LoginName = rdr["LoginName"].ToString();
                        usr.AccountNumber = rdr["AccountNumber"].ToString();
                        usr.Password = rdr["Password"].ToString();
                        usr.Balance = double.Parse(string.Format("{0:N2}", rdr["Balance"].ToString()));
                        usr.CreatedDate = DateTime.Parse(rdr["CreatedDate"].ToString());
                    }
                    //con.Close();
                }
                catch
                {
                    return new Users {
                        ID = 0,
                        LoginName = null,
                        AccountNumber = null,
                        Password = null,
                        Balance = 0,
                        CreatedDate = DateTime.Now
                    };
                }
                finally
                {
                    con.Close();
                }
                
            }
            return usr;
        }

        public List<UserTransactions> GetUserTransactionsByID(string ID)
        {
            List<UserTransactions> lstTrans = new List<UserTransactions>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spUserTransactionsGetAll", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID", ID);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        UserTransactions usr = new UserTransactions();

                        usr.ID = Convert.ToInt32(rdr["ID"]);
                        usr.AccountNumber = rdr["AccountNumber"].ToString();
                        usr.Amount = double.Parse(rdr["Amount"].ToString());
                        usr.RunningBalance = double.Parse(rdr["RunningBalance"].ToString());
                        usr.TransType = rdr["TransType"].ToString();
                        usr.TransDate = DateTime.Parse(rdr["TransDate"].ToString());
                        usr.TransBy = rdr["TransBy"].ToString();
                        usr.Balance = double.Parse(string.Format("{0:N2}", rdr["Balance"].ToString()));

                        lstTrans.Add(usr);
                    }
                    //con.Close();
                }
                catch
                {
                    UserTransactions usr = new UserTransactions();

                    usr.ID = 0;
                    usr.AccountNumber = null;
                    usr.Amount = 0;
                    usr.RunningBalance = 0;
                    usr.TransType = null;
                    usr.TransDate = DateTime.Now;
                    usr.TransBy = null;
                    usr.Balance = 0;

                    lstTrans.Add(usr);
                }
                finally
                {
                    con.Close();
                }
            }
            return lstTrans;
        }

        public void InsertUserTransactions(UserTransactions usrTrans)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    sqlTrans = con.BeginTransaction(IsolationLevel.RepeatableRead);
                    SqlCommand cmd = con.CreateCommand();
                    cmd.Transaction = sqlTrans;
                    //SqlCommand cmd = new SqlCommand("spUserTransactionsInsert", con, sqlTrans);
                    //cmd.CommandType = CommandType.StoredProcedure;

                    //cmd.Parameters.AddWithValue("@ID", usrTrans.ID.ToString());
                    //cmd.Parameters.AddWithValue("@AccountNumber", usrTrans.AccountNumber);
                    //cmd.Parameters.AddWithValue("@Amount", usrTrans.Amount);
                    //cmd.Parameters.AddWithValue("@TransType", usrTrans.TransType);
                    //cmd.Parameters.AddWithValue("@TransDate", usrTrans.TransDate);
                    //cmd.Parameters.AddWithValue("@TransBy", usrTrans.TransBy);
                    cmd.Parameters.AddWithValue("@AccountNumber", usrTrans.AccountNumber);
                    
                    cmd.Parameters.AddWithValue("@TransType", usrTrans.TransType);
                    cmd.Parameters.AddWithValue("@TransDate", usrTrans.TransDate);
                    cmd.Parameters.AddWithValue("@TransBy", usrTrans.TransBy);
                    cmd.Parameters.AddWithValue("@ID", usrTrans.ID.ToString());
                    if (usrTrans.TransType == "Deposit")
                    {
                        cmd.Parameters.AddWithValue("@Amount", usrTrans.Amount);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Amount", usrTrans.Amount * -1);
                    }

                    cmd.CommandText = "Insert into Transactions(AccountNumber,Amount, TransType, TransDate, TransBy)" +
                        "Values (@AccountNumber,@Amount,@TransType, @TransDate,@TransBy)";
                    cmd.ExecuteNonQuery();

                    if (usrTrans.TransType == "Fund Transfer")
                    {
                        cmd.CommandText = "Insert into Transactions(AccountNumber,Amount, TransType, TransDate, TransBy)" +
                            "Values (@AccountNumber,@Amount * -1,@TransType, @TransDate,@TransBy)";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "Update Users set Balance = Balance + @Amount  where AccountNumber = @TransBy";
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "Update Users set Balance = Balance + (@Amount * -1)  where ID = @ID";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = "Update Users set Balance = Balance + @Amount where ID = @ID";
                        cmd.ExecuteNonQuery();
                    }
                    sqlTrans.Commit();
                }
                catch 
                {
                    if (sqlTrans != null)
                    {
                        sqlTrans.Rollback();
                    }
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public Users UserLogin(string LoginName, string Password)
        {
            Users usr = new Users();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spUserLogin", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@LoginName", LoginName);
                    cmd.Parameters.AddWithValue("@Password", Password);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        usr.ID = Convert.ToInt32(rdr["ID"]);
                        usr.LoginName = rdr["LoginName"].ToString();
                        usr.AccountNumber = rdr["AccountNumber"].ToString();
                        usr.Password = rdr["Password"].ToString();
                        usr.Balance = double.Parse(rdr["Balance"].ToString());
                        usr.CreatedDate = DateTime.Parse(rdr["CreatedDate"].ToString());
                    }
                    con.Close();
                    return usr;
                }
                catch 
                {
                    return new Users
                    {
                        ID = 0,
                        LoginName = null,
                        AccountNumber = null,
                        Password = null,
                        Balance = 0,
                        CreatedDate = DateTime.Now
                    };
                }
                finally
                {
                    con.Close();
                }

            }
           
        }

        public bool CheckBalance(string AccountNumber, double CurrBalance)
        {
            double result = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spUserCheckBalanceIfEqual", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        result = double.Parse(rdr["Balance"].ToString());
                    }
                    else
                    {
                        result = 0;
                    }

                    //con.Close();
                    if (result != CurrBalance)
                        return false;
                    return true;
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    con.Close();
                }
                
            }
            
        }
    }
}
