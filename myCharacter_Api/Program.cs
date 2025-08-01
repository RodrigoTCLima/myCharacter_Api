using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;

using myCharacter.Data; 
using myCharacter.Models;
using myCharacter.Services;
using FluentValidation.AspNetCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); 

// --- Configurar a Conexão com o Banco de Dados e o DbContext ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// --- Configurar o ASP.NET Core Identity ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4; // Apenas para desenvolvimento!
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<TokenService>();

builder.Services.AddScoped<DbInitializer>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found."); // <-- CORREÇÃO

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ativa os sistemas de autenticação e autorização.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    // Cria um escopo de serviço temporário para resolver serviços "scoped".
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;

        // Pega o DbContext para poder aplicar as migrations
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        // Pede ao provedor de serviços uma instância do nosso DbInitializer.
        var dbInitializer = serviceProvider.GetRequiredService<DbInitializer>();

        // Chama o método de seeding e espera ele terminar.
        await dbInitializer.SeedAsync();
    }
}
catch (Exception ex)
{
    // Se qualquer coisa der errado durante o seeding, captura a exceção.
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    // Registra o erro detalhado no log para diagnóstico.
    logger.LogError(ex, "An error occurred during database seeding.");
}

app.Run();
