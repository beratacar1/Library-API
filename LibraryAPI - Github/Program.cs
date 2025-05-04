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
var jwtKey = builder.Configuration["Jwt:Key"]; // JWT authenticaiton ayarlar�
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>  // appsettings.json dosyas�ndan JWT �ifresi al�n�r ve byte dizisine �evrilir.
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // JWT ile kullan�c� kimlik do�rulamas� yap�lmas� i�in gerekli ayarlar girildi.
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

// RoleClaimType'� belirleme
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
});

builder.Services.AddAuthorization(); // yetkilendirme hizmeti eklenir.

// JWT END


// Add services to the container.
builder.Services.AddEndpointsApiExplorer(); // Bu Swagger UI i�in gerekli
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library API",
        Version = "v1"
    });

    // JWT deste�i i�in gerekli:
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token giriniz. �rnek: Bearer {token}"
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
        // JSON d�nerken ili�kisel verilerde d�ng� (circular reference) olu�mamas� i�in ReferenceHandler.Preserve se�ilmi�.
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 32;  // Derinlik s�n�r�n� ayarlayabilirsiniz.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // JSON d�n��lerinde d�ng�leri engellemek i�in alternatif bir yap�land�rma.
    });
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LibraryDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IBookRepository, BookRepository>(); // �IBookRepository �a�r�ld���nda onun yerine BookRepository s�n�f�n� kullanmay� sa�lar

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());  //  AutoMapper� uygulamaya ekler
//builder.Services.AddAutoMapper(typeof(GeneralMapping));  // Profil dosyas�n� ekle

//builder.Services.AddControllers().AddJsonOptions(x =>
//  x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); // JSON d�n��lerinde d�ng�leri engellemek i�in alternatif bir yap�land�rma.

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1"); // Swagger dok�mantasyonuna endpoint
        c.RoutePrefix = string.Empty; // Swagger UI ana sayfa olarak gelsin
    });
}



app.UseHttpsRedirection();


// ROL OLU�TURMA 
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "Yazar", "Kullan�c�" }; // Uygulama �al��t���nda Yazar ve Kullan�c� ad�nda iki rol olu�turulur (yoksa)
                                                   //�admin� ve �user� ad�nda iki kullan�c� olu�turulur ve ilgili rollere atan�r.

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
        await userManager.AddToRoleAsync(user, "Kullan�c�");
    }
}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseRequestTiming(); // Kendi middleware'im
app.MapControllers();

app.UseStatusCodePages();

app.Run();


