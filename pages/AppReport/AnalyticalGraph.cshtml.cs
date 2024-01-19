using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Diagnostics;

namespace FeedbackApp.Pages.AppReport
{
    public class AnalyticalGraphModel : PageModel
    {
        public List<string> ChartData { get; set; } = new List<string>();
        public Dictionary<string, int> occurences = new Dictionary<string, int>();
        [BindProperty]
        public string columnName { get; set; }
        [BindProperty]
        public string chartStyle { get; set; }
        public void OnGet()
        {
            Debug.WriteLine("Get function");
            ChartData = GetDataForChart("CategoryID");
            countOccureences();
        }

        public void OnPost()
        {
            Debug.WriteLine("Post function" + columnName);
            ChartData = GetDataForChart(columnName);
            countOccureences();
        }

        private void countOccureences()
        {
            foreach (string chart in ChartData)
            {
                if(occurences.ContainsKey(chart)) 
                { 
                    occurences[chart]++; 
                }
                else
                {
                    occurences.Add(chart, 1);
                }
            }
        }

        private List<string> GetDataForChart(string columnName)
        {
            String connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=Feedback;Integrated Security=True";
            List<string> data = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "SELECT "+columnName+" FROM ReportedTasks";
                SqlCommand command = new SqlCommand(sql, connection);
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(reader.GetString(0));
                    }
                }
            }
            return data;
        }
    }
}
