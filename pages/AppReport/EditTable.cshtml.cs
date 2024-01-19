using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;

namespace FeedbackApp.Pages.AppReport
{
    public class EditTableModel : PageModel
    {
        public ReportedTaskInfo editReport = new ReportedTaskInfo();
        public String errorMessage = "";
        [BindProperty]
        public IFormFileCollection Attachments { get; set; }
        public List<string> listAttachments = new List<string>();
        public void OnGet()
        {
            String id = Request.Query["id"];
            try
            {
                String connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Feedback;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select r1.ID, CategoryID,Title,r1.Description,r3.FactoryName,r2.AppName,r1.Priority,CreationDate from ReportedTasks r1 "
                            +"left join Applications r2 on r1.ApplicationID=r2.ID "
                            +"left join Location r3 on r1.LocationID=r3.FactoryCode "
                            +"where r1.ID=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                editReport.id = reader.GetInt32(0);
                                editReport.Type = reader.GetString(1);
                                editReport.Title = reader.GetString(2);
                                editReport.Description = reader.GetString(3);
                                editReport.Location = reader.GetString(4);
                                editReport.Application = reader.GetString(5);
                                editReport.Priority = "0";
                                editReport.CreationDate = reader.GetDateTime(7);
                            }
                        }
                    }
                    var flag = false;
                    sql = "select FileName from Attachments where ReportID=@id";
                    using(SqlCommand command = new SqlCommand(sql,connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                if (!flag)
                                {
                                    editReport.Attachments = new List<string>() { reader.GetString(0) };
                                    flag = true;
                                }
                                else
                                {
                                    editReport.Attachments.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                    listAttachments = editReport.Attachments;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost() 
        {
            editReport.id = Int32.Parse(Request.Form["id"]);
            editReport.Type = Request.Form["type"];
            editReport.Title = Request.Form["title"];
            editReport.Description= Request.Form["description"];
            editReport.Location= Request.Form["location"];
            editReport.Application = Request.Form["Application"];
            editReport.Priority = Request.Form["Priority"];
            editReport.CreationDate = DateTime.Parse(Request.Form["CreationDate"]);
            foreach(var file in Attachments)
            {
                listAttachments.Add(file.FileName);
            }
            
            editReport.Attachments = listAttachments;

            if(editReport.id == 0 || editReport.Type.Length == 0 || editReport.Title.Length == 0 || editReport.Description.Length == 0 ||
                editReport.Location.Length == 0 || editReport.Application.Length == 0 || editReport.Priority.Length == 0)
            {
                errorMessage = "All fields are required";
                return;
            }

            try
            {
                String connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Feedback;Integrated Security=True";
                String factoryCode, appID;
                int ReportID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT FactoryCode FROM Location WHERE FactoryName = '" + editReport.Location + "'";
                    Debug.WriteLine(sql);
                    SqlCommand command = new SqlCommand(sql, connection);
                    factoryCode = command.ExecuteScalar().ToString();

                    sql = "SELECT ID FROM Applications WHERE AppName = '" + editReport.Application + "'";
                    Debug.WriteLine(sql);
                    command = new SqlCommand(sql, connection);
                    appID = command.ExecuteScalar().ToString();



                    sql = "UPDATE ReportedTasks " +
                            "SET CategoryID=@type, Title=@title, Description=@description, LocationID=@location, ApplicationID=@app, Priority=@priority, LastUpdateDate=@updateDate " +
                            "WHERE ID=@id";
                    using (command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@location", factoryCode);
                        command.Parameters.AddWithValue("@type", editReport.Type);
                        command.Parameters.AddWithValue("@app", appID);
                        command.Parameters.AddWithValue("@title", editReport.Title);
                        command.Parameters.AddWithValue("@description", editReport.Description);
                        command.Parameters.AddWithValue("@priority", editReport.Priority);
                        command.Parameters.AddWithValue("@updateDate", DateTime.Now);
                        command.Parameters.AddWithValue("@id",editReport.id);
                        Debug.WriteLine(sql);
                        command.ExecuteNonQuery();
                    }

                    foreach (var file in listAttachments)
                    {
                        Debug.WriteLine($"{file}");
                        sql = "INSERT INTO ATTACHMENTS (ReportID,FileName) VALUES ('" + editReport.id + "','" + file + "')";
                        Debug.WriteLine(sql);
                        command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/AppReport/EditReportedTask");
        }
    }
}
