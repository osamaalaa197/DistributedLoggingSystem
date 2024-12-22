using DistributedLoggingSystem.Core.IRepository;
using DistributedLoggingSystem.Core.Models;
using DistributedLoggingSystem.EF.Data;
using DistributedLoggingSystem.EF.Repository;
using DistributedLoggingSystem.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region ConnectedDataBase
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DistributedLoggingSystemDbContext>(option =>
option.UseSqlServer(connectionString, e=>e.MigrationsAssembly(typeof(DistributedLoggingSystemDbContext).Assembly.FullName)));
#endregion

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        // Allow requests from any origin, method, and header
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.Configure<S3StorageConfig>(builder.Configuration.GetSection("S3Storage"));
builder.Services.Configure<RabbitMQSenderOptions>(builder.Configuration.GetSection("RabbitMQSenderOptions"));


builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
    .AddEntityFrameworkStores<DistributedLoggingSystemDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
#region DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IRabbitMQLogSender, RabbitMQLogSender>();
builder.Services.AddScoped<ILogConsumerService, LogConsumerService>();
#endregion
builder.Services.AddHttpClient<IS3Repository, S3Repository>();

#region Swagger
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });

}
);
#endregion

#region Jwt
var key = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Secret");
var Issuer = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Issuer");
var Audience = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Audience");
var secret = Encoding.ASCII.GetBytes(key);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(e =>
{
    e.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secret),
        ValidIssuer = Issuer,
        ValidAudience = Audience
    };
});
#endregion
builder.Services.AddAuthorization();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();
