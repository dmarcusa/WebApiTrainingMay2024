using FluentValidation;
using IsssueTracker.Api.Catalog;
using Marten;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer(); // the services that let us use [Authorize section]

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsSoftwareAdmin", policy =>
    {
        policy.RequireRole("SoftwareCenter");
        //policy.RequireUserName("bob@aol.com");
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton<IValidator<CreateCatalogItemRequest>, CreateCatalogItemRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCatalogItemRequestValidator>();

var connectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("Can't start, need a connection string");
builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);
}).UseLightweightSessions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }