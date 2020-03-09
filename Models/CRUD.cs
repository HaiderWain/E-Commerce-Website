using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using Amazonistan.Models;

namespace Amazonistan.Models
{
    public class CRUD
    {
        public static string connectionString = @"Data source=DESKTOP-T174R83\SQLEXPRESS; Initial Catalog=Amazonistan; Integrated Security = True;";

        public static int Login(string email, string password)
        {

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;
            int result = 0;

            try
            {
                cmd = new SqlCommand("Login", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = email;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 16).Value = password;
                cmd.Parameters.Add("@returnCheck", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@returnCheck"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;

        }

        public static int Signup(string name, string email, string password, string number, string address)
        {

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;
            int result = 0;

            try
            {
                cmd = new SqlCommand("Signup", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@userName", SqlDbType.NVarChar, 50).Value = name;
                cmd.Parameters.Add("@userEmail", SqlDbType.NVarChar, 50).Value = email;
                cmd.Parameters.Add("@userPassword", SqlDbType.NVarChar, 50).Value = password;
                cmd.Parameters.Add("@userNumber", SqlDbType.NVarChar, 50).Value = number;
                cmd.Parameters.Add("@userAddress", SqlDbType.NVarChar, 50).Value = address;

                cmd.Parameters.Add("@returnCheck", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                result = Convert.ToInt32(cmd.Parameters["@returnCheck"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;

        }

        public static List<Product> popularProducts()
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("popularProducts", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                List<Product> productList = null;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    productList = new List<Product>();

                    while (reader.Read())
                    {
                        Product p = new Product();

                        p.productID = Convert.ToInt32(reader["ID"]);
                        p.productName = Convert.ToString(reader["Name"]);
                        p.productCategory = Convert.ToString(reader["Category"]);
                        p.productPrice = Convert.ToInt32(reader["Price"]);

                        productList.Add(p);
                    }

                    productList.TrimExcess();

                    reader.Close();
                }

                con.Close();
                return productList;

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());

                return null;
            }
            finally
            {

                con.Close();
            }

        }

        public static List<Product> Search(string productString)
        {

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("Search", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@searchString", SqlDbType.NVarChar, 50).Value = productString;

                List<Product> productList = null;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    productList = new List<Product>();

                    while (reader.Read())
                    {
                        Product p = new Product();

                        p.productID = Convert.ToInt32(reader["ID"]);
                        p.productName = Convert.ToString(reader["Name"]);
                        p.productCategory = Convert.ToString(reader["Category"]);
                        p.productPrice = Convert.ToInt32(reader["Price"]);

                        productList.Add(p);
                    }

                    productList.TrimExcess();

                    reader.Close();

                    return productList;
                }
                else
                {
                    productList = new List<Product>();
                    Product p = new Product();
                    p.productID = 0;
                    p.productName = "N/A";
                    p.productCategory = "N/A";
                    p.productPrice = 0;
                    productList.Add(p);
                    productList.TrimExcess();

                    reader.Close();

                    return productList;
                }


                con.Close();
            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());

                return null;
            }
            finally
            {

                con.Close();
            }
        }
        public static List<currentUser> viewAccountDetails(String email)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("currentUserDetails", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@currentUser", SqlDbType.NVarChar, 50).Value = email;

                List<currentUser> User = null;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    User = new List<currentUser>();

                    while (reader.Read())
                    {
                        currentUser U = new currentUser();

                        U.customerID = Convert.ToInt32(reader["ID"]);
                        U.customerName = Convert.ToString(reader["Name"]);
                        U.customerEmail = Convert.ToString(reader["Email"]);
                        U.customerPassword = Convert.ToString(reader["Password"]);
                        U.customerNumber = Convert.ToString(reader["PhoneNumber"]);
                        U.customerAddress = Convert.ToString(reader["Address"]);
                        U.customerBalance = Convert.ToInt32(reader["Balance"]);

                        User.Add(U);
                    }

                    User.TrimExcess();

                    reader.Close();
                }

                con.Close();
                return User;

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                return null;
            }
            finally
            {
                con.Close();
            }

        }

        public static int changePassword(string email, string password, string newPassword)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;
            int result = 0;

            try
            {
                cmd = new SqlCommand("changePassword", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = email;
                cmd.Parameters.Add("@password", SqlDbType.NVarChar, 16).Value = password;
                cmd.Parameters.Add("@newPassword", SqlDbType.NVarChar, 16).Value = newPassword;

                cmd.Parameters.Add("@returnCheck", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@returnCheck"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;

        }
        public static int itemBuying(string email, int productID)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;
            int result = 0;

            try
            {
                cmd = new SqlCommand("buyingItem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@currentUserEmail", SqlDbType.NVarChar, 50).Value = email;
                cmd.Parameters.Add("@itemID", SqlDbType.Int).Value = productID;
                cmd.Parameters.Add("@returnCheck", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@returnCheck"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;

        }
        public static List<Review> productReviews(int productID)
        {

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;

            try
            {
                cmd = new SqlCommand("readReviews", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@itemID", SqlDbType.Int).Value = productID;

                List<Review> reviewList = null;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reviewList = new List<Review>();

                    while (reader.Read())
                    {
                        Review r = new Review();

                        r.reviewCustomerID = Convert.ToInt32(reader["userID"]);
                        r.reviewProductName = Convert.ToString(reader["Name"]);
                        r.reviewDescription = Convert.ToString(reader["Description"]);
                        r.reviewStars = Convert.ToInt32(reader["Rating"]);


                        reviewList.Add(r);
                    }

                    reviewList.TrimExcess();

                    reader.Close();
                }

                con.Close();
                return reviewList;

            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());

                return null;
            }
            finally
            {
                con.Close();
            }

        }
        public static int itemInsert(String productCategory, String productName, String productPrice, String productAmount)
        {

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;
            int result = 0;

            try
            {
                cmd = new SqlCommand("addNewItem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@productCategory", SqlDbType.NVarChar, 50).Value = productCategory;
                cmd.Parameters.Add("@productName", SqlDbType.NVarChar, 50).Value = productName;
                cmd.Parameters.Add("@productPrice", SqlDbType.Int).Value = productPrice;
                cmd.Parameters.Add("@productAmount", SqlDbType.Int).Value = productAmount;


                cmd.Parameters.Add("@returnCheck", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@returnCheck"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;

        }

        public static int itemDelete(String productName)
        {

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd;
            int result = 0;

            try
            {
                cmd = new SqlCommand("deleteItem", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@productName", SqlDbType.NVarChar, 50).Value = productName;

                cmd.Parameters.Add("@returnCheck", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(cmd.Parameters["@returnCheck"].Value);

            }

            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error" + ex.Message.ToString());
                result = -1; //-1 will be interpreted as "error while connecting with the database."
            }
            finally
            {
                con.Close();
            }
            return result;

        }



    }
}