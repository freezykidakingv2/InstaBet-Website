using SQLite;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.EntityFrameworkCore;

namespace InstaBet_Server.Controllers
{
    public class InstaBetContext : DbContext
    {
        public InstaBetContext(DbContextOptions<InstaBetContext> options) 
            : base(options){}
        public DbSet<UserInformation> UserInformation { get; set; }
    
    }

    [Table("UserInformation")]
    public class UserInformation
    {
        public int ID { get; set; }

        public int InstaCoins { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    [ApiController]
    [Route("api")]
    public class Database : ControllerBase
    {
        private readonly InstaBetContext database;

        public Database(InstaBetContext database)
        {
            this.database = database;
        }

        [HttpGet("createT")]
        public async Task<IActionResult> CreateTable()
        {
            //var result = await database.CreateTableAsync<UserInformation>();

            return Ok();
        }

        [HttpGet("getAllInfo")]
        public async 
            Task<ActionResult<List<UserInformation>>> GetAlltInformationAsync()
        {
            var users = await database.UserInformation
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("getUserInfo/{username}")]
        public async Task<ActionResult<UserInformation>> GetUserInformation(string username)
        {
            var info = await database.UserInformation
                .FirstOrDefaultAsync(u => u.Username == username);

            return Ok(info);
        }

        [HttpPost("addUser")]
        public async Task<IActionResult> AddNewUser([FromBody] UserInformation info)
        {
            try
            {
                Debug.WriteLine("Adding user...");

                database.Add(new UserInformation
                {
                    Username = info.Username,
                    Password = info.Password
                });

                await database.SaveChangesAsync();

                Debug.WriteLine($"User initialized: {info.Username} ,{info.Password}");
                return Ok();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error: Username not initialized {e}");
                return Ok();
                //return StatusCode(500, new 
                //{
                //    error = e.Message,
                //    innerError = e.InnerException?.Message
                //});
            }

        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveUserInformation()
        {
            return Ok(await database.SaveChangesAsync());
        }

        [HttpGet("check/{username}")]
        public async Task<ActionResult<bool>> CheckUser(UserInformation user)
        {
            var check = await database.UserInformation
                .FirstOrDefaultAsync(u => u.Username == user.Username &&
                u.Password == user.Password);

            if (user != null)
            {
                Debug.WriteLine($"User found: {user}, {user.ID}");
                return Ok(true);
            }

            return Ok(false);
        }

        [HttpGet("deleteE")]
        public async Task deleteEmptyRows()
        {
            var query = await database.UserInformation
                .FirstOrDefaultAsync(u => u.Username == null);

            database.UserInformation.Remove(query);
        }

        [HttpGet("deleteUser/{id}")]
        public async Task DeleteUser(int ID)
        {
            var query = await database.UserInformation
                .FirstOrDefaultAsync(u => u.ID == ID);

            database.UserInformation.Remove(query);

            await database.SaveChangesAsync();
        }
    }
}
