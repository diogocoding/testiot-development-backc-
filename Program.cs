using AccessControlAPI.Models;
using AccessControlAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura o Banco de Dados (Supabase)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Supabase")));

// 2. Permite que o Vercel (Frontend) converse com a API sem ser bloqueado (CORS)
builder.Services.AddCors(options => {
    options.AddPolicy("PermitirTudo", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// O trabalhador invisível do Heartbeat
builder.Services.AddHostedService<DeviceMonitorService>();

// 3. Lê a porta dinâmica que a nuvem (Railway) vai exigir
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// 4. Ativa o CORS que criamos ali em cima
app.UseCors("PermitirTudo");

app.UseAuthorization();
app.MapControllers();

app.Run();