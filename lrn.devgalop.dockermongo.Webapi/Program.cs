using lrn.devgalop.dockermongo.Infrastructure.Data.Extensions;
using lrn.devgalop.dockermongo.Core.Extensions;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Extensions;
using lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Extensions;
using lrn.devgalop.dockermongo.Infrastructure.Security.TOTP.Extensions;
using GraphQL.AspNet.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices();
builder.Services.AddMongoDb();
builder.Services.AddAesEncryption();
builder.Services.AddJwtSecurity();
builder.Services.AddTOTP();

builder.Services.AddGraphQL();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseGraphQL();

app.MapControllers();
app.MapHealthChecks("/healthy");
app.Run();

