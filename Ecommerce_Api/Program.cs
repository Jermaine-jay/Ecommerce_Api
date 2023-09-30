using Ecommerce.Data.Seeds;
using Ecommerce.Services.Infrastructure;
using Ecommerce_Api.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManager.Services.Configurations.Jwt;


    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.RegisterDbContext(connectionString);
        builder.Services.RegisterServices();
        builder.Services.ConfigureIdentity();
        //builder.Services.ConfigureAuth(builder.Configuration);
        //builder.Services.ConfigurationBinder(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();

        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy(name: "CorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
});
        });


Settings setting = builder.Configuration.Get<Settings>()!;
builder.Services.AddSingleton(setting);

Cloudinary cloudinary = setting.Cloudinary;
builder.Services.AddSingleton(cloudinary);

JwtConfig jwtConfig = setting.JwtConfig;
builder.Services.AddSingleton(jwtConfig);

builder.Services.ConfigureJWT(jwtConfig);

builder.Services.AddAuthentication()

.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})

.AddFacebook(facebookOptions =>
 {
     facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
     facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
 });

builder.Services.AddMvc();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce Api", Version = "v1" });
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                            Array.Empty<string>()
                    },
        });
});


builder.Services.AddHttpContextAccessor();
var app = builder.Build();

//IApiVersionDescriptionProvider provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("CorsPolicy");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//await app.ProductSeeder();


app.Run();
