using AutoMapper.Internal;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
builder.Services.AddAutoMapper(options => options.Internal().MethodMappingEnabled = false, typeof(Program)); // https://github.com/AutoMapper/AutoMapper/issues/3988
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapSwagger();
app.UseSwaggerUI();

app.MapGroup("/api").MapMinimalEndpoints(typeof(Program));

app.Run();
