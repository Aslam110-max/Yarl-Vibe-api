﻿using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Yarl_Vibe_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenStaffController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public KitchenStaffController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetOrdersData")]
        public JsonResult GetOrdersData()
        {
            string query = "SELECT * FROM Orders";
            DataTable table = new DataTable();
            string connectionString = _configuration.GetConnectionString("yarlVibeDBCon");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
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
