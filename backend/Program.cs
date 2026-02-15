using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Restaurant.API.Data;
using Restaurant.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Npgsql to handle DateTime with legacy timestamp behavior
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add DbContext with snake_case naming
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant POS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
        }
    });
});

// Configure to listen on all network interfaces
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// Verify database connection and ensure schema exists
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("Database schema verified/created successfully");
        
        // Add missing columns to receipt_templates if they don't exist
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS paper_size VARCHAR(10) DEFAULT '80mm';
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_address BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_phone BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_tax_number BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_order_number BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_date BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_order_type BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_table BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_customer BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_payment_method BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_item_code BOOLEAN DEFAULT false;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_modifiers BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_discount_details BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_payment_details BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS show_tips BOOLEAN DEFAULT true;
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS footer_text2 VARCHAR(500);
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS footer_text_ar VARCHAR(500);
                ALTER TABLE receipt_templates ADD COLUMN IF NOT EXISTS footer_text_ar2 VARCHAR(500);
            ");
            Console.WriteLine("Receipt template columns verified");
        }
        catch (Exception) { /* Columns may already exist */ }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Could not setup database: {ex.Message}");
    }
}

// Configure pipeline
app.UseDeveloperExceptionPage(); // Show detailed errors
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/", () => new { status = "healthy", service = "Restaurant POS API", version = "1.0" });

app.Run();
