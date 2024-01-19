using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace FeedbackApp.Pages.AppReport
{
    public class IndexModel : PageModel
    {
        public ReportInfo newReport = new ReportInfo();
        public string errorMessage = "";
        public string successMessage = "";
        [BindProperty]
        public IFormFileCollection Attachments { get; set; }

        

        public async Task<IActionResult> OnPostAsync()
        { 
            newReport.Type = Request.Form["type"];
            newReport.Title = Request.Form["title"];
            newReport.Description = Request.Form["description"];
            newReport.Location = Request.Form["location"];
            newReport.Application = Request.Form["application"];
            Debug.WriteLine(newReport.Type + " " + newReport.Title + " " + newReport.Description + " " + newReport.Location);

            if(newReport.Type.Length==0 || newReport.Title.Length==0 || newReport.Description.Length==0 || newReport.Location.Length==0 || newReport.Application.Length==0)
            {
                errorMessage = "Fill the required fields";
                return RedirectToPage("/AppReport/MainPage");
            }

            
            Debug.WriteLine("************"+Attachments.Count);


            try
            {
                String connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Feedback;Integrated Security=True";
                String factoryCode,appID;
                int ReportID;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT FactoryCode FROM Location WHERE FactoryName = '" + newReport.Location+"'";
                    Debug.WriteLine(sql);
                    SqlCommand command = new SqlCommand(sql, connection);
                    factoryCode = command.ExecuteScalar().ToString();

                    sql = "SELECT ID FROM Applications WHERE AppName = '" + newReport.Application + "'";
                    Debug.WriteLine(sql);
                    command = new SqlCommand(sql, connection);
                    appID = command.ExecuteScalar().ToString();

                    

                    sql = "INSERT INTO ReportedTasks " +
                                 "(LocationID, CategoryID, ApplicationID,Title,Description,CreationDate,Status) OUTPUT INSERTED.ID VALUES" +
                                 "(@location,@type,@app,@title,@description,@date,@status);";
                    using (command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@location", factoryCode);
                        command.Parameters.AddWithValue("@type", newReport.Type);
                        command.Parameters.AddWithValue("@app", appID);
                        command.Parameters.AddWithValue("@title", newReport.Title);
                        command.Parameters.AddWithValue("@description", newReport.Description);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        command.Parameters.AddWithValue("@status", "New");
                        ReportID = (int)command.ExecuteScalar();

                    }

                    foreach (var file in Attachments)
                    {
                        string filename = file.FileName;
                        Debug.WriteLine($"{filename}");
                        sql = "INSERT INTO ATTACHMENTS (ReportID,FileName) VALUES ('"+ ReportID +"','" + filename + "')";
                        Debug.WriteLine(sql);
                        command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return RedirectToPage("/AppReport/MainPage");
            }
            successMessage = "New " + newReport.Type + " reported";
            newReport.Type = "";
            newReport.Title = "";
            newReport.Description = "";
            newReport.Location = "";
            
            Response.Redirect("/AppReport/MainPage");
            return RedirectToPage("/AppReport/MainPage");
        }

    }
    
    public class ReportInfo
    {
        public string Type;
        public string Title;
        public string Description;
        public string Location;
        public string Application;
    }
}
