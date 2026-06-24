using codeHappy.Data.Context;
using Microsoft.EntityFrameworkCore;
using codeHappy.Api.Extensions;
using codeHappy.Api.Endpoints;
using System.IdentityModel.Tokens.Jwt;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


var connectionString = builder.Configuration.GetConnectionString("SupabaseConnection");


builder.Services.AddDbContext<CodeHappyContext>(options =>
                 options.UseNpgsql(connectionString));


//COnfig of Authentication JWT
builder.Services.AddSupabaseAuth(builder.Configuration);

builder.Services.AddAuthorization();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.Run();

