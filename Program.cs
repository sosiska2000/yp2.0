using Microsoft.OpenApi;

using RestAPI.Connect;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Добавляем MVC
builder.Services.AddControllers();

// Добавляем DbContext
builder.Services.AddDbContext<ApplicationDbContext>();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Информационная система учёта оборудования",
        Description = "API для управления инвентаризацией в учебном заведении"
    });

    // Включаем XML комментарии
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Настраиваем Swagger только в Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API v1");
        c.RoutePrefix = "swagger"; // Теперь Swagger будет на /swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();