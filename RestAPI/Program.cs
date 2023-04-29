using FluentValidation;
using FluentValidation.AspNetCore;
using PuppeteerSharp;
using RestAPI.Controllers.AuditRuleController.Requests;
using RestAPI.Controllers.ScanController.Requests;
using RestAPI.Domain.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDomain();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.DisableDataAnnotationsValidation = true;
});

builder.Services.AddScoped<IValidator<InsertRuleRequest>, InsertRuleRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateRuleRequest>, UpdateRuleRequestValidator>();
builder.Services.AddScoped<IValidator<ScanWebsiteRequest>, ScanWebsiteRequestValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapControllers();

using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
browserFetcher.Dispose();

app.Run();