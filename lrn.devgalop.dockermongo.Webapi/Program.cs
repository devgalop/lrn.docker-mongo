using lrn.devgalop.dockermongo.Infrastructure.Data.Extensions;
using lrn.devgalop.dockermongo.Core.Extensions;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices();
builder.Services.AddMongoDb();
builder.Services.AddAesEncryption();

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
app.MapControllers();
app.MapHealthChecks("/healthy");
app.Run();

