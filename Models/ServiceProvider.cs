using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace EMPLOYEE_MANAGEMENT.Models
{
    public class ServiceProvider
    {
        string CS = ConfigurationManager.AppSettings["Mycon"];


        public object GeneralOperation(string ReturnType, string Query, string TypeOfQuery, List<GeneralIOModel> list)
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand();
                if (TypeOfQuery == "STORE_PROCEDURE")
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else if (TypeOfQuery == "TEXT")
                {
                    cmd.CommandType = CommandType.Text;
                }

                if (list != null)
                {
                    if (list.Count > 0)
                    {
                        foreach (GeneralIOModel model in list)
                        {
                            cmd.Parameters.AddWithValue("@" + model.Name, model.Value);
                        }

                    }
                }
                cmd.Connection = con;
                cmd.CommandText = Query;


                try
                {

                    if (ReturnType == "DATA")
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                        return dt;
                    }
                    else if (ReturnType == "STATUS")
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                            int i = cmd.ExecuteNonQuery();
                            if (i > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                   Console.WriteLine(Query + ex.Message.ToString());
                    
                }
                finally
                {
                    con.Close();
                }


                return null;
            }
        }

    }
}