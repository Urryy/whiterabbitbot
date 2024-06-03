using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using WhiteRabbitTelegram;
using WhiteRabbitTelegram.Repository;
using WhiteRabbitTelegram.Service;
using WhiteRabbitTelegram.Visitor;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel();

string conn = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationDatabaseContext>(opt => opt.UseSqlServer(conn));

string hangfireConn = builder.Configuration.GetConnectionString("HangfireDefaultConnection")!;
builder.Services.AddHangfire(configuration => configuration
    .UseSqlServerStorage(hangfireConn));
builder.Services.AddHangfireServer();

builder.Services.AddTransient<TelegramBot>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ITelegramBotService, TelegramBotService>();
builder.Services.AddTransient<INFTRepository, NFTRepository>();
builder.Services.AddTransient<ITokenWCRepository, TokenWCRepository>();
builder.Services.AddTransient<IBaseVisitor, BaseVisitor>();
builder.Services.AddTransient<IHttpClientService, HttpClientService>();
builder.Services.AddTransient<ICronJobService, CronJobService>();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.Services.GetRequiredService<ICronJobService>().DoJob("0 */15 * * * *");
app.Services.GetRequiredService<TelegramBot>().Start();

app.UseRouting();

app.MapControllers();

app.Run();
