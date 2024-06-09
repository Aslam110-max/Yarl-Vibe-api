using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace Yarl_Vibe_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateOrderStatusController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UpdateOrderStatusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("UpdateOrderStatus")]
        public JsonResult UpdateOrderStatus([FromBody] JObject requestData)
        {
            int orderID = (int)requestData["orderID"];
            string foodStatus = requestData["foodStatus"].ToString();
            string query = "UPDATE Orders SET FoodStatus = @foodStatus WHERE OrderID = @orderID;";
            string connectionString = _configuration.GetConnectionString("yarlVibeDBCon");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters
                    command.Parameters.AddWithValue("@foodStatus", foodStatus);
                    command.Parameters.AddWithValue("@orderID", orderID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new JsonResult("Order status updated successfully!");
                        }
                        else
                        {
                            return new JsonResult("No rows were updated.");
                        }
                    }
                    catch (Exception ex)
                    {
                        return new JsonResult($"Error updating order status: {ex.Message}");
                    }
                }
            }
        }
    }
}
