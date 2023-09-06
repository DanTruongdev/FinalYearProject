using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineShopping.Data;
using OnlineShopping.Hubs;
using OnlineShopping.Libraries.Models;
using OnlineShopping.Libraries.Services;
using OnlineShopping.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//Allow Cross origin for API
builder.Services.AddCors(p => p.AddPolicy(name: "corsapp", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

//HttpContext
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

//For Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseLazyLoadingProxies()
     .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
          
//For Identity Framework
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//For Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
}).AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["GoogleAuthentication:ClientID"];
        googleOptions.ClientSecret = configuration["GoogleAuthentication:ClientSecret"];
    }
    
);


//Add config for required email
builder.Services.Configure<IdentityOptions>(
    options => options.SignIn.RequireConfirmedEmail = true
   
);
//Add config for required email
builder.Services.Configure<IdentityOptions>(
    options => options.SignIn.RequireConfirmedPhoneNumber = false

);

// Forgot password 
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromMinutes(10));   


//Add Email Configs
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailService, EmailSevice>();

//Add PhoneNums Configs
var phoneNumsConfig = configuration.GetSection("SmsConfiguration").Get<SmsConfiguration>();
builder.Services.AddSingleton(phoneNumsConfig);
builder.Services.AddScoped<ISMSService, SmsService>();

//Maximum upload file size
builder.WebHost.ConfigureKestrel(options =>
    options.Limits.MaxRequestBodySize = 5*1024*1024
);;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//Add SignalR
builder.Services.AddSignalR();

//Config swagger
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
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
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseForwardedHeaders();
app.UseRouting();
app.UseCors("corsapp"); 
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<SignalHub>("/signalhub");
app.Run();
