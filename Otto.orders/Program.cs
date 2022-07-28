using Otto.orders;
using Otto.orders.Models;
using Otto.orders.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddNpgsql<OrderDb>(ConfigHelper.GetConnectionString());

builder.Services.AddHttpContextAccessor();
builder.Services.AddHostedService<QueueService>();
builder.Services.AddScoped<AccessTokenService>();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<QueueTasks>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
