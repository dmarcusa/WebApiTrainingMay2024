using FluentValidation;
using IsssueTracker.Api.Catalog;
using IssueTracker.Api.Catalog;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer(); // the services that let us use [Authorize section]
builder.Services.AddScoped<IAuthorizationHandler, ShouldBeCreatorOfCatalogItemRequirementHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsSoftwareAdmin", policy =>
    {
        policy.RequireRole("SoftwareCenter");
        policy.AddRequirements(new ShouldBeCreatorToDeleteCatalogItemRequirement());
        //policy.RequireUserName("bob@aol.com");
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

// Add services to the container.

//builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header with bearer token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",
                Name = "Bearer ",
                In = ParameterLocation.Header
            },
            []
        }
    });
}); // this will add the stuff to generate an OpenApi specification.
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