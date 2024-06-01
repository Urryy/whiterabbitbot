using System.Globalization;
using TonSdk.Client;
using WhiteRabbitTelegram.Command;
using WhiteRabbitTelegram.Records;

namespace WhiteRabbitTelegram.Entity;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long TelegramId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public DateTime DateUpdated { get; set; } = DateTime.UtcNow;
    public string? TelegramWallet { get; set; } = default!;
    public int? CountNFT { get; set; } = default!;
    public decimal TokensWhiteCoin { get; set; } = 0;
    public int Referrals { get; set;} = 0;
    public string CurrentCommand { get; set; } = UserCommands.StartCommand;
    public string LastCommand { get; set; } = UserCommands.StartCommand;
    public string OwnReferralId { get; set; }
    public bool IsFirstSign { get; set; } = true;

    protected User()
    {

    }

    public User(long telegramId, string name) : this()
    {
        TelegramId = telegramId;
        Name = name;
        OwnReferralId = Guid.NewGuid().ToString().Split("-").Last();
    }

    public void SetBalance(JettonsRecord? jettons)
    {
        if (jettons != null && jettons.balances.Count != 0)
        {
            var whiteCoinJetton = jettons.balances.FirstOrDefault(item => item.jetton.address.Contains("0:7e876e10133376dd9d443a51315d3cc36336b855d29545f27ffa77c131aff107"));
            if (whiteCoinJetton != null)
            {
                var value = decimal.Parse(whiteCoinJetton.balance, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                TokensWhiteCoin = (value / 1000000000);
            }
        }
    }
}

