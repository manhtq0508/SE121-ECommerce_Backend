using ECommerceApp.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
const string BearerSecurityScheme = JwtBearerDefaults.AuthenticationScheme;

builder.Configuration.AddSerilogEnvironmentOverrides();
builder.Host.UseApplicationSerilog();

builder.Services.AddApplicationServices(builder.Configuration, BearerSecurityScheme);

var app = builder.Build();

await app.MigrateDatabaseAsync();
app.UseApplicationPipeline(BearerSecurityScheme);

app.Run();
