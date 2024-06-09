using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Yarl_Vibe_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrderDetailsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetOrderDetails/{orderID}")]
        public JsonResult GetOrderDetails(int orderID)
        {
            string query = "SELECT * FROM OrderedFoods WHERE OrderID = @orderID";
            DataTable table = new DataTable();
            string connectionString = _configuration.GetConnectionString("yarlVibeDBCon");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderID", orderID);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        table.Load(reader);
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        // Handle error
                        return new JsonResult($"An error occurred: {ex.Message}");
                    }
                }
            }

            return new JsonResult(table);
        }
    }
}
