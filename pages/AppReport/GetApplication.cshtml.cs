using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace FeedbackApp.Pages.AppReport
{
    public class GetApplicationModel : PageModel
    {
        public JsonResult OnGet(string location)
        {
            List<string> applications = GetApplicationsForLocation(location);
            return new JsonResult(applications);
        }

        private List<string> GetApplicationsForLocation(string location)
        {
            List<string> applications = new List<string>();
            String connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Feedback;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "select a3.AppName from [Feedback].[dbo].[AppsPerFactory] a1 left join [Feedback].[dbo].Location a2 on a1.FactoryCode=a2.FactoryCode left join [Feedback].[dbo].Applications a3 on a1.AppID=a3.ID where a2.FactoryName = '" + location + "'";
                Debug.WriteLine(sql);
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(reader.GetString(0));
                    }
                }
                connection.Close();
            }
            return applications;
        }
    }
}
