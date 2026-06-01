var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Adicionar o serviço de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirPainelReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // A URL exata do seu Vite
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

    
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

// 2. Ativar a política de CORS ANTES do UseAuthorization
app.UseCors("PermitirPainelReact");

app.UseAuthorization();
app.MapControllers();
app.Run();