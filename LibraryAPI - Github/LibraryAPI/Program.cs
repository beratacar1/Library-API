using BusinessLayer.Mapping;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using LibraryAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// JWT START
var jwtKey = builder.Configuration["Jwt:Key"]; // JWT authenticaiton ayarlarý
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>  // appsettings.json dosyasýndan JWT þifresi alýnýr ve byte dizisine çevrilir.
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // JWT ile kullanýcý kimlik doðrulamasý yapýlmasý için gerekli ayarlar girildi.
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        RoleClaimType = ClaimTypes.Role
    };
});

// RoleClaimType'ý belirleme
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
});

builder.Services.AddAuthorization(); // yetkilendirme hizmeti eklenir.

// JWT END


// Add services to the container.
builder.Services.AddEndpointsApiExplorer(); // Bu Swagger UI için gerekli
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1"
    });

    // JWT desteði için gerekli:
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token giriniz. Örnek: Bearer {token}"
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
            new string[] {}
        }
            });
});




builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // JSON dönerken iliþkisel verilerde döngü (circular reference) oluþmamasý için ReferenceHandler.Preserve seçilmiþ.
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 32;  // Derinlik sýnýrýný ayarlayabilirsiniz.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // JSON dönüþlerinde döngüleri engellemek için alternatif bir yapýlandýrma.
    });
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LibraryDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IBookRepository, BookRepository>(); // “IBookRepository çaðrýldýðýnda onun yerine BookRepository sýnýfýný kullanmayý saðlar

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());  //  AutoMapperý uygulamaya ekler
//builder.Services.AddAutoMapper(typeof(GeneralMapping));  // Profil dosyasýný ekle

//builder.Services.AddControllers().AddJsonOptions(x =>
//  x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // JSON dönüþlerinde döngüleri engellemek için alternatif bir yapýlandýrma.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1"); // Swagger dokümantasyonuna endpoint
        c.RoutePrefix = string.Empty; // Swagger UI ana sayfa olarak gelsin
    });
}



app.UseHttpsRedirection();


// ROL OLUÞTURMA 
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "Yazar", "Kullanýcý" }; // Uygulama çalýþtýðýnda Yazar ve Kullanýcý adýnda iki rol oluþturulur (yoksa)
                                                   //“admin” ve “user” adýnda iki kullanýcý oluþturulur ve ilgili rollere atanýr.

    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var admin = new ApplicationUser { UserName = "admin", Email = "admin@example.com" };
    if (await userManager.FindByNameAsync(admin.UserName) == null)
    {
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Yazar");
    }

    var user = new ApplicationUser { UserName = "user", Email = "user@example.com" };
    if (await userManager.FindByNameAsync(user.UserName) == null)
    {
        await userManager.CreateAsync(user, "User123!");
        await userManager.AddToRoleAsync(user, "Kullanýcý");
    }
}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseRequestTiming(); // Kendi middleware'im
app.MapControllers();

app.UseStatusCodePages();

app.Run();


