using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Yarl_Vibe_api.Controllers.LoginController
{
    public class LoginController : Controller
    {
        private readonly string _connectionString;

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("yarlVibeDBCon");
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string query = "SELECT UserID, UserRole FROM Users WHERE Username = @username AND PasswordHash = @passwordHash";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", request.Username);
                    command.Parameters.AddWithValue("@passwordHash", request.PasswordHash);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            string userRole = reader.GetString(1);
                            return Ok(new { UserId = userId, UserRole = userRole });
                        }
                        else
                        {
                            return Unauthorized("Invalid username or password");
                        }
                    }
                }
            }
        }
    }

public class LoginRequest
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}
}
