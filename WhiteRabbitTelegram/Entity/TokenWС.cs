namespace WhiteRabbitTelegram.Entity;

public class TokenWС
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Wallet { get; set; }
    public decimal Tokens { get; set; }
    public DateTime DateEarn { get; set; } = DateTime.UtcNow;

    protected TokenWС()
    {

    }

    public TokenWС(decimal tokens, string wallet) : this()
    {
        Tokens = tokens;
        Wallet = wallet;
    }
}
