using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using RestAPI.Connect;
using RestAPI.Services;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Добавь это для работы с CORS (чтобы WPF мог подключаться)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWpfClient",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:7000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>();

// Регистрируем все сервисы
builder.Services.AddScoped<OborudovanieService>();
builder.Services.AddScoped<PolzovateliService>();
builder.Services.AddScoped<AuditoriiService>();
builder.Services.AddScoped<NapravleniyaService>();
builder.Services.AddScoped<StatusyService>();
builder.Services.AddScoped<TipyOborudovaniaService>();
builder.Services.AddScoped<VidyModeleiService>();
builder.Services.AddScoped<RazrabotchikiService>();
builder.Services.AddScoped<ProgrammyService>();
builder.Services.AddScoped<SetevyeNastroikiService>();
builder.Services.AddScoped<TipyRaskhodnykhMaterialovService>();
builder.Services.AddScoped<RaskhodnyeMaterialyService>();
builder.Services.AddScoped<KharakteristikiMaterialovService>();
builder.Services.AddScoped<InventarizatsiaService>();
builder.Services.AddScoped<InventarizatsiaDetaliService>();
builder.Services.AddScoped<LogiOshibokService>();
builder.Services.AddScoped<DokumentyService>();

// Настройки для загрузки файлов
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
    options.MemoryBufferThreshold = 1024 * 1024;
}); 

builder.Services.AddEndpointsApiExplorer();

// Настройка Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Информационная система учёта оборудования",
        Description = "API для управления инвентаризацией в учебном заведении"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Создаем директорию для логов
var logsDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logsDir))
{
    Directory.CreateDirectory(logsDir);
}

var app = builder.Build();

// Настройка конвейера запросов
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Добавляем CORS перед авторизацией
app.UseCors("AllowWpfClient");

app.UseAuthorization();

app.MapControllers();

// Глобальная обработка ошибок
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        // Логируем ошибку
        using var scope = app.Services.CreateScope();
        var logService = scope.ServiceProvider.GetRequiredService<LogiOshibokService>();
        logService.LogError("Middleware", ex.Message);

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Внутренняя ошибка сервера");
    }
});

app.Run();