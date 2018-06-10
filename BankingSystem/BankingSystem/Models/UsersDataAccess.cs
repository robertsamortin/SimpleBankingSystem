﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BankingSystem.Models
{
    public class UsersDataAccess
    {
        string connectionString = "Server =.\\SQLEXPRESS01;Database=Banking;Integrated Security = true;";  
  
        //register new user    
        public void AddUser(Users usr)  
        {  
            using (SqlConnection con = new SqlConnection(connectionString))  
            {  
                SqlCommand cmd = new SqlCommand("spUserInsert", con);  
                cmd.CommandType = CommandType.StoredProcedure;  
                
                cmd.Parameters.AddWithValue("@LoginName", usr.LoginName);  
                cmd.Parameters.AddWithValue("@AccountNumber", usr.AccountNumber);  
                cmd.Parameters.AddWithValue("@Password", usr.Password);  
                cmd.Parameters.AddWithValue("@Balance", usr.Balance);
                cmd.Parameters.AddWithValue("@CreatedDate", usr.CreatedDate);

                con.Open();  
                cmd.ExecuteNonQuery();  
                con.Close();  
            }  
        }  
  
        //update by user id  
        public void UpdateUser(Users usr)  
        {  
            using (SqlConnection con = new SqlConnection(connectionString))  
            {  
                SqlCommand cmd = new SqlCommand("spUpdateEmployee", con);  
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", usr.ID);
                cmd.Parameters.AddWithValue("@LoginName", usr.LoginName);
                cmd.Parameters.AddWithValue("@AccountNumber", usr.AccountNumber);
                cmd.Parameters.AddWithValue("@Password", usr.Password);
                cmd.Parameters.AddWithValue("@Balance", usr.Balance);
                cmd.Parameters.AddWithValue("@CreatedDate", usr.CreatedDate);

                con.Open();  
                cmd.ExecuteNonQuery();  
                con.Close();  
            }  
        }

        //Login  
        public Users UserLogin(string LoginName, string Password)
        {
            Users usr = new Users();

            using (SqlConnection con = new SqlConnection(connectionString))
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
            }
            return usr;
        }
        
        //get user by account number  
        public Users GetUserByAccountNumber(string AccountNumber)
        {
            Users usr = new Users();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserGetByAccountNumber", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);

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
                con.Close();
            }
            return usr;
        }

        //check loginname if exist  
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


        //get users transactions by account number
        public List<UserTransactions> GetUserTransactionsByAccountNumber(string AccountNumber)
        {
            List<UserTransactions> lstTrans = new List<UserTransactions>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserTransactionsGetAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AccountNumber", AccountNumber);

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
                con.Close();
            }
            return lstTrans;
        }

        //insert users transactions    
        public void InsertUserTransactions(UserTransactions usrTrans)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserTransactionsInsert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@AccountNumber", usrTrans.AccountNumber);
                cmd.Parameters.AddWithValue("@Amount", usrTrans.Amount);
                cmd.Parameters.AddWithValue("@TransType", usrTrans.TransType);
                cmd.Parameters.AddWithValue("@TransDate", usrTrans.TransDate);
                cmd.Parameters.AddWithValue("@TransBy", usrTrans.TransBy);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}