using ChatRoom.API.Endpoints;
using ChatRoom.API.Helpers;
using ChatRoom.API.Mappers;
using ChatRoom.API.Middleware;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddAutoMapper(typeof(ChatEventMappingProfile));

services.AddValidatorsFromAssemblyContaining<Program>();

services.RegisterChatEventDependencies();

services.AddCors(options => 
    options.AddDefaultPolicy(corsPolicyBuilder => 
        corsPolicyBuilder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.SeedDatabase();

app.MapEndpoints();

app.Run();