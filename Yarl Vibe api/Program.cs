using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Yarl_Vibe_api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("yarlVibeDBCon"));
});

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JSON Serializer
builder.Services.AddControllers().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson(
    options=>options.SerializerSettings.ContractResolver=new DefaultContractResolver());

var app = builder.Build();

//Enable CORS
app.UseCors(c=>c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapIdentityApi<IdentityUser>();

app.MapControllers();

// Role and User Seeding
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "KitchenStaff", "Waiter", "Cashier" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var userNames = new[] { "admin1", "kitchenStaff1", "waiter1", "cashier1" };
    var emails = new[] { "admin1@gmail.com", "kitchenStaff1@gmail.com", "waiter1@gmail.com" , "cashier1@gmail.com" };
    var passwords = new[] { "@Admin1test", "@kitchenStaff1test", "@waiter1test", "@cashier1test" };
    var roles = new[] { "Admin", "KitchenStaff", "Waiter", "Cashier" };

    for (int i = 0; i < userNames.Length; i++)
    {
        if (await userManager.FindByEmailAsync(emails[i]) == null)
        {
            var user = new IdentityUser { UserName = userNames[i], Email = emails[i] };
            var result = await userManager.CreateAsync(user, passwords[i]);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, roles[i]);
            }
        }
    }
}


app.Run();
