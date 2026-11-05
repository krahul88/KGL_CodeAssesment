using Microsoft.EntityFrameworkCore;
using ProductionPlantEvaluation.Application.Interfaces;
using ProductionPlantEvaluation.Application.Services;
using ProductionPlantEvaluation.Domain.DTO;
using ProductionPlantEvaluation.Domain.Interfaces;
using ProductionPlantEvaluation.Infrastructure;
using ProductionPlantEvaluation.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));
builder.Services.AddScoped<IRuleRepository, RuleRepository>();
builder.Services.AddScoped<IEvaluationLogRepository, EvaluationLogRepository>();

builder.Services.AddScoped<IRuleEvaluator, RuleEvaluator>();

builder.Services.AddScoped<
    IOrderEvaluationService,
    OrderEvaluationService>();
builder.Services.AddScoped<IRuleCacheService, RuleCacheService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();


