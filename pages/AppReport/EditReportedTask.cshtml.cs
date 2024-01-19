using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;

namespace FeedbackApp.Pages.AppReport
{
    public class EditReportedTaskModel : PageModel
    {
        public List<ReportedTaskInfo> listReports = new List<ReportedTaskInfo>();
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Feedback;Integrated Security=True";
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "select ID, CategoryID,Title,Description,LocationID,ApplicationID,Priority,CreationDate from ReportedTasks";
                    using(SqlCommand  command = new SqlCommand(sql, connection))
                    {
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listReports.Add(new ReportedTaskInfo
                                {
                                    id = reader.GetInt32(0),
                                    Type = reader.GetString(1),
                                    Title = reader.GetString(2),
                                    Description = reader.GetString(3),
                                    Location = reader.GetString(4),
                                    Application = reader.GetString(5),
                                    Priority = "0",
                                    CreationDate = reader.GetDateTime(7)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class ReportedTaskInfo
    {
        public int id;
        public string Type;
        public string Title;
        public string Description;
        public string Location;
        public string Application;
        public string Priority;
        public DateTime CreationDate;
        public List<string> Attachments;

    }
}
