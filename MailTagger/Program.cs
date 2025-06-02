using MailTagger.Middlewares;
using MailTagger.Services;
using MailTagger.Services.Interfaces;
using MailTagger.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("OpenAiSettings"));
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("GeminiSettings"));

builder.Services.AddHttpClient();

builder.Services.AddScoped<IMailTaggerService, MailTaggerService>();

builder.Services.AddScoped<OpenAiService>();
builder.Services.AddScoped<GeminiService>();

// Add services to the container.

builder.Services.AddControllers();
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

app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

await app.RunAsync();
