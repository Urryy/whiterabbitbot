using Hangfire;
using Telegram.Bot;
using WhiteRabbitTelegram.Extension;
using WhiteRabbitTelegram.Keyboard;
using WhiteRabbitTelegram.Repository;

namespace WhiteRabbitTelegram.Service;

public static class RecurringJobs
{
    public static string NotificationEarnJob = "NotificationEarnJob";
}

public class CronJobService : ICronJobService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public CronJobService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public void DoJob(string cronExpression)
    {
        RecurringJob.AddOrUpdate(RecurringJobs.NotificationEarnJob, () => NotifactionEarn(), cronExpression);
    }

    public async Task NotifactionEarn()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var bot = new TelegramBotClient(_configuration["TelegramToken"]!);
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var users = await userRepository.GetAllUsers();
            foreach (var user in users)
            {
                if(user.DateUpdated.AddHours(6) <= DateTime.Now)
                {
                    try
                    {
                        await bot.SendTextMessageAsync(user.TelegramId, "Пора добывать монеты", replyMarkup: InlineKeyboardButtonMessage.GetButtonEarnCoins());
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }
    }
}
