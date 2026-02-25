using Microsoft.EntityFrameworkCore;
using RestAPI.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddMvc(option => option.EnableEndpointRouting = true);
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v1",
        Title = "Аудитории",
        Description = "Управление информацией об аудиториях"
    });
    option.SwaggerDoc("v2", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v2",
        Title = "Оборудование",
        Description = "Управление информацией об оборудовании"
    });
    option.SwaggerDoc("v3", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v3",
        Title = "Расходные материалы",
        Description = "Управление информацией о расходных материалах"
    });
    option.SwaggerDoc("v4", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v4",
        Title = "Пользователи",
        Description = "Управление пользователями системы"
    });
    option.SwaggerDoc("v5", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v5",
        Title = "Справочники",
        Description = "Управление справочниками (типы, статусы, направления)"
    });
    option.SwaggerDoc("v6", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v6",
        Title = "Инвентаризация",
        Description = "Управление инвентаризацией"
    });
    option.SwaggerDoc("v7", new Microsoft.OpenApi.OpenApiInfo
    {
        Version = "v7",
        Title = "Сетевые настройки",
        Description = "Управление сетевыми настройками оборудования"
    });

    // Включаем XML комментарии (опционально)
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // option.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseRouting();

app.MapControllers(); // Вместо UseEndpoints

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Аудитории");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Оборудование");
    c.SwaggerEndpoint("/swagger/v3/swagger.json", "Расходные материалы");
    c.SwaggerEndpoint("/swagger/v4/swagger.json", "Пользователи");
    c.SwaggerEndpoint("/swagger/v5/swagger.json", "Справочники");
    c.SwaggerEndpoint("/swagger/v6/swagger.json", "Инвентаризация");
    c.SwaggerEndpoint("/swagger/v7/swagger.json", "Сетевые настройки");
});

app.Run();