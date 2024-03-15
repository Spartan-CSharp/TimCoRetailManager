using System.Configuration;
using System.Text;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using TRMApi.Data;

using TRMDataManager.Library.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCors(policy =>
{
	policy.AddPolicy("OpenCorsPolicy", opt =>
		opt.AllowAnyOrigin()
		.AllowAnyHeader()
		.AllowAnyMethod());
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = "JwtBearer";
	options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", jwtBearerOptions =>
	{
		jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Secrets:SecurityKey"))),
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.FromMinutes(5)
		};
	});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
	setup.SwaggerDoc(
		"v1",
		new OpenApiInfo
		{
			Title = "TimCo Retail Manager API",
			Version = "v1"
		});
});

// Personal Services
builder.Services.AddTransient<IInventoryData, InventoryData>();
builder.Services.AddTransient<IProductData, ProductData>();
builder.Services.AddTransient<ISaleData, SaleData>();
builder.Services.AddTransient<IUserData, UserData>();
builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
	app.UseDeveloperExceptionPage();
	app.UseMigrationsEndPoint();
	app.UseSwagger();
	app.UseSwaggerUI(x =>
	{
		x.SwaggerEndpoint("/swagger/v1/swagger.json", "TimCo API v1");
	});
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("OpenCorsPolicy");
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
