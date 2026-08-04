using codeHappy.Data.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using codeHappy.Api.Extensions;
using codeHappy.Api.Endpoints;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using codeHappy.Business.Interfaces;
using codeHappy.Business.Services;
using codeHappy.Api.Middlewares;
using codeHappy.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


var connectionString = builder.Configuration.GetConnectionString("SupabaseConnection");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<CodeHappyContext>(options =>
                options.UseNpgsql(dataSource));


//Services
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISpaceService, SpaceService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ISnippetService, SnippetService>();
builder.Services.AddScoped<IBlocksService, BlockService>();
builder.Services.AddScoped<IImagesService, ImagesService>();
builder.Services.AddScoped<IShareService, ShareService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddValidatorsFromAssembly(typeof(SpaceService).Assembly);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


builder.Services.AddClientCors(builder.Configuration);

//Config of Authentication JWT
builder.Services.AddSupabaseAuth(builder.Configuration);
builder.Services.AddCloudinary(builder.Configuration);
builder.Services.AddAuthorization();


builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter( JsonNamingPolicy.CamelCase, allowIntegerValues: false));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseCors("FrontendCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapSpaceEndpoints();
app.MapGroupEndpoints();
app.MapSnippetEndpoints();
app.MapBlockEndpoints();
app.MapImageEndpoints();
app.MapShareEndpoints();
app.MapCommentEndpoints();
app.MapHealthEndpoint();

app.Run();

